using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Media;
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
            var logger = LoggerFactory.createLogger(args);
            var logo = new StringBuilder();
            logo.AppendLine(@"  ____ _____ _____ _  ______   ___ _____");
            logo.AppendLine(@" / ___| ____| ____| |/ / __ ) / _ \\_  _|");
            logo.AppendLine(@"| |  _|  _| |  _| | ' /|  _ \| | | || |");
            logo.AppendLine(@"| |_| | |___| |___| . \| |_) | |_| || |");
            logo.AppendLine(@" \____|_____|_____|_|\_\____/ \___/ |_|");
            logo.AppendLine("=========================================");
            Console.WriteLine(logo.ToString());
            logger.Information("[Geekbot] Starting...");
            new Program().MainAsync(args, logger).GetAwaiter().GetResult();
        }

        private async Task MainAsync(string[] args, ILogger logger)
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
            catch (Exception e)
            {
                logger.Fatal(e, "[Redis] Redis Connection Failed");
                Environment.Exit(102);
            }

            if (args.Length != 0 && args.Contains("--migrate"))
            {
                Console.WriteLine("\nYou are about to migrate the database, this will overwrite an already migrated database?");
                Console.Write("Are you sure [y:N]: ");
                var migrateDbConfirm = Console.ReadKey();
                Console.WriteLine();
                if (migrateDbConfirm.Key == ConsoleKey.Y)
                {
                    logger.Warning("[Geekbot] Starting Migration");
                    await MigrateDatabaseToHash();
                    logger.Warning("[Geekbot] Finished Migration");
                }
                else
                {
                    logger.Information("[Geekbot] Not Migrating db");
                }
            }
            
            token = redis.StringGet("discordToken");
            if (token.IsNullOrEmpty)
            {
                Console.Write("Your bot Token: ");
                var newToken = Console.ReadLine();
                redis.StringSet("discordToken", newToken);
                redis.StringSet("Game", "Ping Pong");
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
            var errorHandler = new ErrorHandler(logger);
            
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
                    await client.SetGameAsync(redis.StringGet("Game"));
                    logger.Information($"[Geekbot] Now Connected to {client.Guilds.Count} Servers");

                    logger.Information("[Geekbot] Registering Stuff");

                    client.MessageReceived += HandleCommand;
                    client.MessageReceived += HandleMessageReceived;
                    client.UserJoined += HandleUserJoined;
                    await commands.AddModulesAsync(Assembly.GetEntryAssembly());
                    services.AddSingleton(commands);
                    services.AddSingleton<DiscordSocketClient>(client);
                    this.servicesProvider = services.BuildServiceProvider();
                    logger.Information("[Geekbot] Done and ready for use\n");
                }
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Could not connect to discord...");
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
            
            var channel = (SocketGuildChannel) message.Channel;
            
            await redis.HashIncrementAsync($"{channel.Guild.Id}:Messages", message.Author.Id.ToString());
            await redis.HashIncrementAsync($"{channel.Guild.Id}:Messages", 0.ToString());

            if (message.Author.IsBot) return;
            logger.Information($"[Message] {channel.Guild.Name} - {message.Channel} - {message.Author.Username} - {message.Content}");
        }

        private async Task HandleUserJoined(SocketGuildUser user)
        {
            if (!user.IsBot)
            {
                var message = redis.HashGet($"{user.Guild.Id}:Settings", "WelcomeMsg");
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
        
        // temporary db migration script
        private Task MigrateDatabaseToHash()
        {
            foreach (var key in redis.Multiplexer.GetServer("127.0.0.1", 6379).Keys(6))
            {
                var keyParts = key.ToString().Split("-");
                if (keyParts.Length == 2 || keyParts.Length == 3)
                {
                    logger.Verbose($"Migrating key {key}");
                    var stuff = new List<string>();
                    stuff.Add("messages");
                    stuff.Add("karma");
                    stuff.Add("welcomeMsg");
                    stuff.Add("correctRolls");
                    if(stuff.Contains(keyParts[keyParts.Length - 1]))
                    {
                        var val = redis.StringGet(key);
                        ulong.TryParse(keyParts[0], out ulong guildId);
                        ulong.TryParse(keyParts[1], out ulong userId);

                        switch (keyParts[keyParts.Length - 1])
                        {
                            case "messages":
                                redis.HashSet($"{guildId}:Messages", new HashEntry[] { new HashEntry(userId.ToString(), val) });
                                break;
                            case "karma":
                                redis.HashSet($"{guildId}:Karma", new HashEntry[] { new HashEntry(userId.ToString(), val) });
                                break;
                            case "correctRolls":
                                redis.HashSet($"{guildId}:Rolls", new HashEntry[] { new HashEntry(userId.ToString(), val) });
                                break;
                            case "welcomeMsg":
                                redis.HashSet($"{guildId}:Settings", new HashEntry[] { new HashEntry("WelcomeMsg", val) });
                                break;
                        }
                        //redis.KeyDelete(key);

                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}