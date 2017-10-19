using System;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IDatabase _redis;
        private IServiceCollection _services;
        private IServiceProvider _servicesProvider;
        private RedisValue _token;
        private ILogger _logger;
        private IUserRepository _userRepository;
        private string[] _args;
        private bool _firstStart = false;

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
            _logger = logger;
            _args = args;
            logger.Information("[Geekbot] Initing Stuff");

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true
            });
            _client.Log += DiscordLogger;
            _commands = new CommandService();

            try
            {
                var redisMultiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379");
                _redis = redisMultiplexer.GetDatabase(6);
                logger.Information($"[Redis] Connected to db {_redis.Database}");
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
                    await DbMigration.MigrateDatabaseToHash(_redis, logger);
                    logger.Warning("[Geekbot] Finished Migration");
                }
                else
                {
                    logger.Information("[Geekbot] Not Migrating db");
                }
            }
            
            _token = _redis.StringGet("discordToken");
            if (_token.IsNullOrEmpty)
            {
                Console.Write("Your bot Token: ");
                var newToken = Console.ReadLine();
                _redis.StringSet("discordToken", newToken);
                _redis.StringSet("Game", "Ping Pong");
                _token = newToken;
                _firstStart = true;
            }

            _services = new ServiceCollection();
            
            _userRepository = new UserRepository(_redis, logger);
            var errorHandler = new ErrorHandler(logger);
            var randomClient = new Random();
            var fortunes = new FortunesProvider(randomClient, logger);
            var mediaProvider = new MediaProvider(randomClient, logger);
            var malClient = new MalClient(_redis, logger);
            
            _services.AddSingleton<IErrorHandler>(errorHandler);
            _services.AddSingleton(_redis);
            _services.AddSingleton<ILogger>(logger);
            _services.AddSingleton<IUserRepository>(_userRepository);
            _services.AddSingleton(randomClient);
            _services.AddSingleton<IFortunesProvider>(fortunes);
            _services.AddSingleton<IMediaProvider>(mediaProvider);
            _services.AddSingleton<IMalClient>(malClient);

            logger.Information("[Geekbot] Connecting to Discord");

            await Login();

            await Task.Delay(-1);
        }

        private async Task Login()
        {
            try
            {
                await _client.LoginAsync(TokenType.Bot, _token);
                await _client.StartAsync();
                var isConneted = await isConnected();
                if (isConneted)
                {
                    await _client.SetGameAsync(_redis.StringGet("Game"));
                    _logger.Information($"[Geekbot] Now Connected to {_client.Guilds.Count} Servers");

                    _logger.Information("[Geekbot] Registering Stuff");
                    
                    await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
                    _services.AddSingleton(_commands);
                    _services.AddSingleton<DiscordSocketClient>(_client);
                    _servicesProvider = _services.BuildServiceProvider();
                    
                    var handlers = new Handlers(_client, _logger, _redis, _servicesProvider, _commands, _userRepository);
                    
                    _client.MessageReceived += handlers.RunCommand;
                    _client.MessageReceived += handlers.UpdateStats;
                    _client.MessageDeleted += handlers.MessageDeleted;
                    _client.UserJoined += handlers.UserJoined;
                    _client.UserUpdated += handlers.UserUpdated;
                    _client.UserLeft += handlers.UserLeft;

                    if (_firstStart || _args.Contains("--reset"))
                    {
                        _logger.Information("[Geekbot] Finishing setup");
                        await FinishSetup();
                        _logger.Information("[Geekbot] Setup finished");
                    }
                    if (!_args.Contains("--disable-api"))
                    {
                        startWebApi();
                    }
                    
                    _logger.Information("[Geekbot] Done and ready for use\n");
                }
            }
            catch (Exception e)
            {
                _logger.Fatal(e, "Could not connect to discord...");
                Environment.Exit(103);
            }
        }

        private async Task<bool> isConnected()
        {
            while (!_client.ConnectionState.Equals(ConnectionState.Connected))
                await Task.Delay(25);
            return true;
        }

        private void startWebApi()
        {
            _logger.Information("[API] Starting Webserver");
            var webApiUrl = new Uri("http://localhost:12995");
            new WebHostBuilder()
                .UseSetting(WebHostDefaults.ApplicationKey, Constants.Name)
                .UseUrls(webApiUrl.ToString())
                .UseKestrel()
                .UseStartup<WebConfig>()
                .Build()
                .Run();
            _logger.Information($"[API] Webserver now running on {webApiUrl}");
        }

        private async Task<Task> FinishSetup()
        {
            var appInfo = await _client.GetApplicationInfoAsync();
            _redis.StringSet("botOwner", appInfo.Owner.Id);

            var req = HttpWebRequest.Create(appInfo.IconUrl);
            using (Stream stream = req.GetResponse().GetResponseStream() )
            {
                await _client.CurrentUser.ModifyAsync(Avatar => new Image(stream));
            }
            return Task.CompletedTask;
        }

        private Task DiscordLogger(LogMessage message)
        {
            var logMessage = $"[{message.Source}] {message.Message}";
            switch (message.Severity)
            {
                case LogSeverity.Verbose:
                    _logger.Verbose(logMessage);
                    break;
                case LogSeverity.Debug:
                    _logger.Debug(logMessage);
                    break;
                case LogSeverity.Info:
                    _logger.Information(logMessage);
                    break;
                case LogSeverity.Critical:
                case LogSeverity.Error:
                case LogSeverity.Warning:
                    _logger.Error(message.Exception, logMessage);
                    break;
                default:
                    _logger.Information($"{logMessage} --- {message.Severity}");
                    break;
            }
            return Task.CompletedTask;
        }
    }
}