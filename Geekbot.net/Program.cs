using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true
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

            if (args.Contains("--migrate"))
            {
                Console.WriteLine("\nYou are about to migrate the database, this will overwrite an already migrated database?");
                Console.Write("Are you sure [y:N]: ");
                var migrateDbConfirm = Console.ReadKey();
                Console.WriteLine();
                if (migrateDbConfirm.Key == ConsoleKey.Y)
                {
                    logger.Warning("[Geekbot] Starting Migration");
                    await DbMigration.MigrateDatabaseToHash(redis, logger);
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
                firstStart = true;
            }

            services = new ServiceCollection();
            
            userRepository = new UserRepository(redis, logger);
            var errorHandler = new ErrorHandler(logger);
            var randomClient = new Random();
            var fortunes = new FortunesProvider(randomClient, logger);
            var mediaProvider = new MediaProvider(randomClient, logger);
            var malClient = new MalClient(redis, logger);
            var levelCalc = new LevelCalc();
            
            services.AddSingleton<IErrorHandler>(errorHandler);
            services.AddSingleton(redis);
            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<IUserRepository>(userRepository);
            services.AddSingleton<ILevelCalc>(levelCalc);
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
                    logger.Information($"[Geekbot] Now Connected to {client.Guilds.Count} Servers");

                    logger.Information("[Geekbot] Registering Stuff");
                    
                    await commands.AddModulesAsync(Assembly.GetEntryAssembly());
                    services.AddSingleton(commands);
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
            redis.StringSet("botOwner", appInfo.Owner.Id);

            var req = HttpWebRequest.Create(appInfo.IconUrl);
            using (Stream stream = req.GetResponse().GetResponseStream() )
            {
                await client.CurrentUser.ModifyAsync(Avatar => new Image(stream));
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