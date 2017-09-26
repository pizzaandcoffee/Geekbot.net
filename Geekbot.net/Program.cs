using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net
{
    internal class Program
    {
        private DiscordSocketClient client;
        private CommandService commands;
        private IDatabase redis;
        private IServiceCollection services;
        private IServiceProvider servicesProvider;
        private RedisValue token;
        private ILogger logger;
        private ulong botOwnerId;

        private static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile("Logs/geekbot-{Date}.txt", shared: true)
                .CreateLogger();

            var logo = new StringBuilder();
            logo.AppendLine(@"  ____ _____ _____ _  ______   ___ _____");
            logo.AppendLine(@" / ___| ____| ____| |/ / __ ) / _ \\_  _|");
            logo.AppendLine(@"| |  _|  _| |  _| | ' /|  _ \| | | || |");
            logo.AppendLine(@"| |_| | |___| |___| . \| |_) | |_| || |");
            logo.AppendLine(@" \____|_____|_____|_|\_\____/ \___/ |_|");
            logo.AppendLine("=========================================");
            Console.WriteLine(logo.ToString());
            logger.Information("[Geekbot] Starting...");
            new Program().MainAsync(logger).GetAwaiter().GetResult();
        }

        private async Task MainAsync(ILogger logger)
        {
            this.logger = logger;
            logger.Information("[Geekbot] Initing Stuff");

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            client.Log += DiscordLogger;
            commands = new CommandService();

            try
            {
                var redisMultiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379");
                redis = redisMultiplexer.GetDatabase(6);
                logger.Information($"[Redis] Connected to db {redis.Database}");
            }
            catch (Exception)
            {
                logger.Information("Start Redis pls...");
                Environment.Exit(102);
            }

            token = redis.StringGet("discordToken");
            if (token.IsNullOrEmpty)
            {
                Console.Write("Your bot Token: ");
                var newToken = Console.ReadLine();
                redis.StringSet("discordToken", newToken);
                token = newToken;
            }

            var botOwner = redis.StringGet("botOwner");
            if (botOwner.IsNullOrEmpty)
            {
                Console.Write("Bot Owner User ID: ");
                botOwner = Console.ReadLine();
                redis.StringSet("botOwner", botOwner);
            }
            botOwnerId = (ulong) botOwner;

            services = new ServiceCollection();
            var RandomClient = new Random();
            var fortunes = new FortunesProvider(RandomClient, logger);
            var checkEmImages = new CheckEmImageProvider(RandomClient, logger);
            var pandaImages = new PandaProvider(RandomClient, logger);
            IErrorHandler errorHandler = new ErrorHandler(logger);
            services.AddSingleton<IErrorHandler>(errorHandler);
            services.AddSingleton(redis);
            services.AddSingleton(RandomClient);
            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<IFortunesProvider>(fortunes);
            services.AddSingleton<ICheckEmImageProvider>(checkEmImages);
            services.AddSingleton<IPandaProvider>(pandaImages);

            logger.Information("[Geekbot] Connecting to Discord");

            await Login();

            await Task.Delay(-1);
        }

        private async Task Login()
        {
            try
            {
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
                var isConneted = await isConnected();
                if (isConneted)
                {
                    await client.SetGameAsync("Ping Pong");
                    logger.Information($"[Geekbot] Now Connected to {client.Guilds.Count} Servers");

                    logger.Information("[Geekbot] Registering Stuff");

                    client.MessageReceived += HandleCommand;
                    client.MessageReceived += HandleMessageReceived;
                    client.UserJoined += HandleUserJoined;
                    await commands.AddModulesAsync(Assembly.GetEntryAssembly());
                    services.AddSingleton(commands);
                    this.servicesProvider = services.BuildServiceProvider();
                    logger.Information("[Geekbot] Done and ready for use\n");
                }
            }
            catch (AggregateException)
            {
                logger.Information("Could not connect to discord...");
                Environment.Exit(103);
            }
        }

        private async Task<bool> isConnected()
        {
            while (!client.ConnectionState.Equals(ConnectionState.Connected))
                await Task.Delay(25);
            return true;
        }

        private async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            if (message.Author.IsBot) return;
            var argPos = 0;
            var lowCaseMsg = message.ToString().ToLower();
            if (lowCaseMsg.StartsWith("ping"))
            {
                await message.Channel.SendMessageAsync("pong");
                return;
            }
            if (lowCaseMsg.StartsWith("hui"))
            {
                await message.Channel.SendMessageAsync("hui!!!");
                return;
            }
            if (!(message.HasCharPrefix('!', ref argPos) ||
                  message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            var context = new CommandContext(client, message);
            var commandExec = commands.ExecuteAsync(context, argPos, servicesProvider);
        }

        private async Task HandleMessageReceived(SocketMessage messsageParam)
        {
            var message = messsageParam;
            if (message == null) return;

            var statsRecorder = new StatsRecorder(message, redis);
            var userRec = statsRecorder.UpdateUserRecordAsync();
            var guildRec = statsRecorder.UpdateGuildRecordAsync();

            if (message.Author.Id == client.CurrentUser.Id) return;
            var channel = (SocketGuildChannel) message.Channel;
            logger.Information($"[Message] {channel.Guild.Name} - {message.Channel} - {message.Author.Username} - {message.Content}");
            await userRec;
            await guildRec;
        }

        private async Task HandleUserJoined(SocketGuildUser user)
        {
            if (!user.IsBot)
            {
                var message = redis.StringGet(user.Guild.Id + "-welcomeMsg");
                if (!message.IsNullOrEmpty)
                {
                    message = message.ToString().Replace("$user", user.Mention);
                    await user.Guild.DefaultChannel.SendMessageAsync(message);
                }
            }
        }

        private Task DiscordLogger(LogMessage message)
        {
            var logMessage = $"[{message.Source}] {message.Message}";
            switch (message.Severity)
            {
                case LogSeverity.Verbose:
                    logger.Verbose(logMessage);
                    break;
                case LogSeverity.Debug:
                    logger.Debug(logMessage);
                    break;
                case LogSeverity.Info:
                    logger.Information(logMessage);
                    break;
                case LogSeverity.Critical:
                case LogSeverity.Error:
                case LogSeverity.Warning:
                    logger.Error(message.Exception, logMessage);
                    break;
                default:
                    logger.Information($"{logMessage} --- {message.Severity.ToString()}");
                    break;
            }
            return Task.CompletedTask;
        }
    }
}