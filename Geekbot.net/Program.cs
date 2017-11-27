using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Media;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Hosting.Self;
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
        private IUserRepository userRepository;
        private string[] args;
        private bool firstStart = false;

        private static void Main(string[] args)
        {
            var logo = new StringBuilder();
            logo.AppendLine(@"  ____ _____ _____ _  ______   ___ _____");
            logo.AppendLine(@" / ___| ____| ____| |/ / __ ) / _ \\_  _|");
            logo.AppendLine(@"| |  _|  _| |  _| | ' /|  _ \| | | || |");
            logo.AppendLine(@"| |_| | |___| |___| . \| |_) | |_| || |");
            logo.AppendLine(@" \____|_____|_____|_|\_\____/ \___/ |_|");
            logo.AppendLine("=========================================");
            Console.WriteLine(logo.ToString());
            var logger = LoggerFactory.createLogger(args);
            logger.Information("[Geekbot] Starting...");
            try
            {
                new Program().MainAsync(args, logger).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                logger.Fatal(e, "[Geekbot] RIP");
            }
        }

        private async Task MainAsync(string[] args, ILogger logger)
        {
            this.logger = logger;
            this.args = args;
            logger.Information("[Geekbot] Initing Stuff");

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
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
            
            token = redis.StringGet("discordToken");
            if (token.IsNullOrEmpty)
            {
                Console.Write("Your bot Token: ");
                var newToken = Console.ReadLine();
                redis.StringSet("discordToken", newToken);
                redis.StringSet("Game", "Ping Pong");
                token = newToken;
                firstStart = true;
            }

            services = new ServiceCollection();
            
            userRepository = new UserRepository(redis, logger);
            var randomClient = new Random();
            var fortunes = new FortunesProvider(randomClient, logger);
            var mediaProvider = new MediaProvider(randomClient, logger);
            var malClient = new MalClient(redis, logger);
            var levelCalc = new LevelCalc();
            var emojiConverter = new EmojiConverter();
            var audioUtils = new AudioUtils();
            
            services.AddSingleton(redis);
            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<IUserRepository>(userRepository);
            services.AddSingleton<ILevelCalc>(levelCalc);
            services.AddSingleton<IEmojiConverter>(emojiConverter);
            services.AddSingleton<IAudioUtils>(audioUtils);
            services.AddSingleton(randomClient);
            services.AddSingleton<IFortunesProvider>(fortunes);
            services.AddSingleton<IMediaProvider>(mediaProvider);
            services.AddSingleton<IMalClient>(malClient);

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
                    logger.Information($"[Geekbot] Now Connected as {client.CurrentUser.Username} to {client.Guilds.Count} Servers");

                    logger.Information("[Geekbot] Registering Stuff");
                    var translationHandler = new TranslationHandler(client.Guilds, redis, logger);
                    var errorHandler = new ErrorHandler(logger, translationHandler);
                    await commands.AddModulesAsync(Assembly.GetEntryAssembly());
                    services.AddSingleton(commands);
                    services.AddSingleton<IErrorHandler>(errorHandler);
                    services.AddSingleton<ITranslationHandler>(translationHandler);
                    services.AddSingleton<DiscordSocketClient>(client);
                    servicesProvider = services.BuildServiceProvider();
                    
                    var handlers = new Handlers(client, logger, redis, servicesProvider, commands, userRepository);
                    
                    client.MessageReceived += handlers.RunCommand;
                    client.MessageReceived += handlers.UpdateStats;
                    client.MessageDeleted += handlers.MessageDeleted;
                    client.UserJoined += handlers.UserJoined;
                    client.UserUpdated += handlers.UserUpdated;
                    client.UserLeft += handlers.UserLeft;

                    if (firstStart || args.Contains("--reset"))
                    {
                        logger.Information("[Geekbot] Finishing setup");
                        await FinishSetup();
                        logger.Information("[Geekbot] Setup finished");
                    }
                    if (!args.Contains("--disable-api"))
                    {
                        startWebApi();
                    }
                    
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

        private void startWebApi()
        {
            logger.Information("[API] Starting Webserver");
            var webApiUrl = new Uri("http://localhost:12995");
            new NancyHost(webApiUrl).Start();
            logger.Information($"[API] Webserver now running on {webApiUrl}");
        }

        private async Task<Task> FinishSetup()
        {
            var appInfo = await client.GetApplicationInfoAsync();
            logger.Information($"[Setup] Just a moment while i setup everything {appInfo.Owner.Username}");
            try
            {
                redis.StringSet("botOwner", appInfo.Owner.Id);
                var req = HttpWebRequest.Create(appInfo.IconUrl);
                using (var stream = req.GetResponse().GetResponseStream())
                {
                    await client.CurrentUser.ModifyAsync(User =>
                    {
                        User.Avatar = new Image(stream);
                        User.Username = appInfo.Name.ToString();
                    });
                }
                logger.Information($"[Setup] Everything done, enjoy!");
            }
            catch (Exception e)
            {
                logger.Warning(e, "[Setup] Oha, it seems like something went wrong while running the setup");
                logger.Warning("[Setup] Geekbot will work never the less, some features might be disabled though");
            }
            return Task.CompletedTask;
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
                    if (logMessage.Contains("VOICE_STATE_UPDATE")) break;
                    logger.Error(message.Exception, logMessage);
                    break;
                default:
                    logger.Information($"{logMessage} --- {message.Severity}");
                    break;
            }
            return Task.CompletedTask;
        }
    }
}