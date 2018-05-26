using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Database;
using Geekbot.net.Lib;
using Geekbot.net.Lib.AlmostRedis;
using Geekbot.net.Lib.Audio;
using Geekbot.net.Lib.Clients;
using Geekbot.net.Lib.Converters;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.GlobalSettings;
using Geekbot.net.Lib.Levels;
using Geekbot.net.Lib.Localization;
using Geekbot.net.Lib.Logger;
using Geekbot.net.Lib.Media;
using Geekbot.net.Lib.ReactionListener;
using Geekbot.net.Lib.UserRepository;
using Microsoft.Extensions.DependencyInjection;
using WikipediaApi;

namespace Geekbot.net
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private DatabaseContext _database;
        private IGlobalSettings _globalSettings;
        private IServiceCollection _services;
        private IServiceProvider _servicesProvider;
        private string _token;
        private GeekbotLogger _logger;
        private IUserRepository _userRepository;
        private RunParameters _runParameters;
        private IAlmostRedis _redis;

        private static void Main(string[] args)
        {
            RunParameters runParameters = null;
            Parser.Default.ParseArguments<RunParameters>(args)
                .WithParsed(e => runParameters = e)
                .WithNotParsed(_ => Environment.Exit(GeekbotExitCode.InvalidArguments.GetHashCode()));
            
            var logo = new StringBuilder();
            logo.AppendLine(@"  ____ _____ _____ _  ______   ___ _____");
            logo.AppendLine(@" / ___| ____| ____| |/ / __ ) / _ \\_  _|");
            logo.AppendLine(@"| |  _|  _| |  _| | ' /|  _ \| | | || |");
            logo.AppendLine(@"| |_| | |___| |___| . \| |_) | |_| || |");
            logo.AppendLine(@" \____|_____|_____|_|\_\____/ \___/ |_|");
            logo.AppendLine($"Version {Constants.BotVersion()} ".PadRight(41, '='));
            Console.WriteLine(logo.ToString());
            var sumologicActive = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GEEKBOT_SUMO"));
            var logger = new GeekbotLogger(runParameters, sumologicActive);
            logger.Information(LogSource.Geekbot, "Starting...");
            try
            {
                new Program().MainAsync(runParameters, logger).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                logger.Error(LogSource.Geekbot, "RIP", e);
            }
        }

        private async Task MainAsync(RunParameters runParameters, GeekbotLogger logger)
        {
            _logger = logger;
            _runParameters = runParameters;
            logger.Information(LogSource.Geekbot, "Initing Stuff");
            var discordLogger = new DiscordLogger(logger);

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            });
            _client.Log += discordLogger.Log;
            _commands = new CommandService();

            _database = new DatabaseInitializer(runParameters, logger).Initzialize();
            _globalSettings = new GlobalSettings(_database);
            
            try
            {
                _redis = new AlmostRedis(logger, runParameters);
                _redis.Connect();
            }
            catch (Exception e)
            {
                logger.Error(LogSource.Redis, "Redis Connection Failed", e);
                Environment.Exit(GeekbotExitCode.RedisConnectionFailed.GetHashCode());
            }
            
            _token = runParameters.Token ?? _globalSettings.GetKey("DiscordToken");
            if (string.IsNullOrEmpty(_token))
            {
                Console.Write("Your bot Token: ");
                var newToken = Console.ReadLine();
                _globalSettings.SetKey("DiscordToken", newToken);
                _globalSettings.SetKey("Game", "Ping Pong");
                _token = newToken;
            }

            _services = new ServiceCollection();
            
            _userRepository = new UserRepository(_database, logger);
            var fortunes = new FortunesProvider(logger);
            var mediaProvider = new MediaProvider(logger);
            var malClient = new MalClient(_globalSettings, logger);
            var levelCalc = new LevelCalc();
            var emojiConverter = new EmojiConverter();
            var mtgManaConverter = new MtgManaConverter();
            var wikipediaClient = new WikipediaClient();
            var audioUtils = new AudioUtils();
            
            _services.AddSingleton<IAlmostRedis>(_redis);
            _services.AddSingleton<IUserRepository>(_userRepository);
            _services.AddSingleton<IGeekbotLogger>(logger);
            _services.AddSingleton<ILevelCalc>(levelCalc);
            _services.AddSingleton<IEmojiConverter>(emojiConverter);
            _services.AddSingleton<IFortunesProvider>(fortunes);
            _services.AddSingleton<IMediaProvider>(mediaProvider);
            _services.AddSingleton<IMalClient>(malClient);
            _services.AddSingleton<IMtgManaConverter>(mtgManaConverter);
            _services.AddSingleton<IWikipediaClient>(wikipediaClient);
            _services.AddSingleton<IAudioUtils>(audioUtils);
            _services.AddSingleton<DatabaseContext>(_database);
            _services.AddSingleton<IGlobalSettings>(_globalSettings);

            logger.Information(LogSource.Geekbot, "Connecting to Discord");

            await Login();

            await Task.Delay(-1);
        }

        private async Task Login()
        {
            try
            {
                await _client.LoginAsync(TokenType.Bot, _token);
                await _client.StartAsync();
                var isConneted = await IsConnected();
                if (isConneted)
                {
                    await _client.SetGameAsync(_globalSettings.GetKey("Game"));
                    _logger.Information(LogSource.Geekbot, $"Now Connected as {_client.CurrentUser.Username} to {_client.Guilds.Count} Servers");

                    _logger.Information(LogSource.Geekbot, "Registering Stuff");
                    var translationHandler = new TranslationHandler(_client.Guilds, _database, _logger);
                    var errorHandler = new ErrorHandler(_logger, translationHandler, _runParameters.ExposeErrors);
                    var reactionListener = new ReactionListener(_redis.Db);
                    await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
                    _services.AddSingleton(_commands);
                    _services.AddSingleton<IErrorHandler>(errorHandler);
                    _services.AddSingleton<ITranslationHandler>(translationHandler);
                    _services.AddSingleton(_client);
                    _services.AddSingleton<IReactionListener>(reactionListener);
                    _servicesProvider = _services.BuildServiceProvider();
                    
                    var handlers = new Handlers(_database, _client, _logger, _redis, _servicesProvider, _commands, _userRepository, reactionListener);
                    
                    _client.MessageReceived += handlers.RunCommand;
                    _client.MessageReceived += handlers.UpdateStats;
                    _client.MessageDeleted += handlers.MessageDeleted;
                    _client.UserJoined += handlers.UserJoined;
                    _client.UserUpdated += handlers.UserUpdated;
                    _client.UserLeft += handlers.UserLeft;
                    _client.ReactionAdded += handlers.ReactionAdded;
                    _client.ReactionRemoved += handlers.ReactionRemoved;

                    var webserver = _runParameters.DisableApi ? Task.Delay(10) : StartWebApi();
                    
                    _logger.Information(LogSource.Geekbot, "Done and ready for use");

                    await webserver;
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Could not connect to Discord", e);
                Environment.Exit(GeekbotExitCode.CouldNotLogin.GetHashCode());
            }
        }

        private async Task<bool> IsConnected()
        {
            while (!_client.ConnectionState.Equals(ConnectionState.Connected))
                await Task.Delay(25);
            return true;
        }

        private Task StartWebApi()
        {
            _logger.Information(LogSource.Api, "Starting Webserver");
            WebApi.WebApiStartup.StartWebApi(_logger, _runParameters, _commands, _database);
            return Task.CompletedTask;
        }
    }
}