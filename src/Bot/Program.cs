using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.Bot.Handlers;
using Geekbot.Core;
using Geekbot.Core.Converters;
using Geekbot.Core.Database;
using Geekbot.Core.DiceParser;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.GlobalSettings;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.Highscores;
using Geekbot.Core.KvInMemoryStore;
using Geekbot.Core.Levels;
using Geekbot.Core.Logger;
using Geekbot.Core.Media;
using Geekbot.Core.RandomNumberGenerator;
using Geekbot.Core.ReactionListener;
using Geekbot.Core.UserRepository;
using Geekbot.Core.WikipediaClient;
using Geekbot.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using Constants = Geekbot.Core.Constants;

namespace Geekbot.Bot
{
    internal class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private DatabaseInitializer _databaseInitializer;
        private IGlobalSettings _globalSettings;
        private IServiceProvider _servicesProvider;
        private GeekbotLogger _logger;
        private IUserRepository _userRepository;
        private RunParameters _runParameters;
        private IReactionListener _reactionListener;
        private IGuildSettingsManager _guildSettingsManager;

        private static async Task Main(string[] args)
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
            var logger = new GeekbotLogger(runParameters);
            logger.Information(LogSource.Geekbot, "Starting...");
            try
            {
                await new Program().Start(runParameters, logger);
            }
            catch (Exception e)
            {
                logger.Error(LogSource.Geekbot, "RIP", e);
            }
        }

        private async Task Start(RunParameters runParameters, GeekbotLogger logger)
        {
            _logger = logger;
            _runParameters = runParameters;
            
            logger.Information(LogSource.Geekbot, "Connecting to Database");
            var database = ConnectToDatabase();
            _globalSettings = new GlobalSettings(database);

            if (!runParameters.DisableGateway)
            {
                logger.Information(LogSource.Geekbot, "Connecting to Discord");
                SetupDiscordClient();
                await Login();
                _logger.Information(LogSource.Geekbot, $"Now Connected as {_client.CurrentUser.Username} to {_client.Guilds.Count} Servers");
                await _client.SetGameAsync(_globalSettings.GetKey("Game"));
            }
            
            RegisterSentry();

            _logger.Information(LogSource.Geekbot, "Loading Dependencies and Handlers");
            RegisterDependencies();
            if (!runParameters.DisableGateway) await RegisterHandlers();

            if (runParameters.DisableApi)
            {
                _logger.Information(LogSource.Geekbot, "Done and ready for use");
                await Task.Delay(-1);
            }
            else
            {
                _logger.Information(LogSource.Api, "Starting Web API");
                StartWebApi();
            }
        }

        private async Task Login()
        {
            try
            {
                var token = await GetToken();
                await _client.LoginAsync(TokenType.Bot, token);
                await _client.StartAsync();
                while (!_client.ConnectionState.Equals(ConnectionState.Connected)) await Task.Delay(25);
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Could not connect to Discord", e);
                Environment.Exit(GeekbotExitCode.CouldNotLogin.GetHashCode());
            }
        }

        private DatabaseContext ConnectToDatabase()
        {
            _databaseInitializer = new DatabaseInitializer(_runParameters, _logger);
            var database = _databaseInitializer.Initialize();
            database.Database.EnsureCreated();
            if(!_runParameters.InMemory) database.Database.Migrate();

            return database;
        }

        private async Task<string> GetToken()
        {
            var token = _runParameters.Token ?? _globalSettings.GetKey("DiscordToken");
            if (string.IsNullOrEmpty(token))
            {
                Console.Write("Your bot Token: ");
                var newToken = Console.ReadLine();
                await _globalSettings.SetKey("DiscordToken", newToken);
                await _globalSettings.SetKey("Game", "Ping Pong");
                token = newToken;
            }

            return token;
        }

        private void SetupDiscordClient()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000,
                ExclusiveBulkDelete = true
            });
            
            var discordLogger = new DiscordLogger(_logger);
            _client.Log += discordLogger.Log;
        }

        private void RegisterDependencies()
        {
            var services = new ServiceCollection();
            
            _userRepository = new UserRepository(_databaseInitializer.Initialize(), _logger);
            _reactionListener = new ReactionListener(_databaseInitializer.Initialize());
            _guildSettingsManager = new GuildSettingsManager(_databaseInitializer.Initialize());
            var fortunes = new FortunesProvider(_logger);
            var levelCalc = new LevelCalc();
            var emojiConverter = new EmojiConverter();
            var mtgManaConverter = new MtgManaConverter();
            var wikipediaClient = new WikipediaClient();
            var randomNumberGenerator = new RandomNumberGenerator();
            var mediaProvider = new MediaProvider(_logger, randomNumberGenerator);
            var kvMemoryStore = new KvInInMemoryStore();
            var errorHandler = new ErrorHandler(_logger, _runParameters, () => Geekbot.Core.Localization.Internal.SomethingWentWrong);
            var diceParser = new DiceParser(randomNumberGenerator);
            
            services.AddSingleton(_userRepository);
            services.AddSingleton<IGeekbotLogger>(_logger);
            services.AddSingleton<ILevelCalc>(levelCalc);
            services.AddSingleton<IEmojiConverter>(emojiConverter);
            services.AddSingleton<IFortunesProvider>(fortunes);
            services.AddSingleton<IMediaProvider>(mediaProvider);
            services.AddSingleton<IMtgManaConverter>(mtgManaConverter);
            services.AddSingleton<IWikipediaClient>(wikipediaClient);
            services.AddSingleton<IRandomNumberGenerator>(randomNumberGenerator);
            services.AddSingleton<IKvInMemoryStore>(kvMemoryStore);
            services.AddSingleton<IGlobalSettings>(_globalSettings);
            services.AddSingleton<IErrorHandler>(errorHandler);
            services.AddSingleton<IDiceParser>(diceParser);
            services.AddSingleton<IReactionListener>(_reactionListener);
            services.AddSingleton<IGuildSettingsManager>(_guildSettingsManager);
            services.AddTransient<IHighscoreManager>(e => new HighscoreManager(_databaseInitializer.Initialize(), _userRepository));
            services.AddTransient(e => _databaseInitializer.Initialize());
            if (!_runParameters.DisableGateway) services.AddSingleton(_client);
            
            _servicesProvider = services.BuildServiceProvider();
        }
        
        private async Task RegisterHandlers()
        {
            var applicationInfo = await _client.GetApplicationInfoAsync();
            
            _commands = new CommandService();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _servicesProvider);
            
            var commandHandler = new CommandHandler(_databaseInitializer.Initialize(), _client, _logger, _servicesProvider, _commands, applicationInfo, _guildSettingsManager);
            var userHandler = new UserHandler(_userRepository, _logger, _databaseInitializer.Initialize(), _client);
            var reactionHandler = new ReactionHandler(_reactionListener);
            var statsHandler = new StatsHandler(_logger, _databaseInitializer.Initialize());
            var messageDeletedHandler = new MessageDeletedHandler(_databaseInitializer.Initialize(), _logger, _client);
                    
            _client.MessageReceived += commandHandler.RunCommand;
            _client.MessageDeleted += messageDeletedHandler.HandleMessageDeleted;
            _client.UserJoined += userHandler.Joined;
            _client.UserUpdated += userHandler.Updated;
            _client.UserLeft += userHandler.Left;
            _client.ReactionAdded += reactionHandler.Added;
            _client.ReactionRemoved += reactionHandler.Removed;
            if (!_runParameters.InMemory) _client.MessageReceived += statsHandler.UpdateStats;
        }

        private void StartWebApi()
        {
            var highscoreManager = new HighscoreManager(_databaseInitializer.Initialize(), _userRepository);
            WebApiStartup.StartWebApi(_servicesProvider, _logger, _runParameters, _commands, _databaseInitializer.Initialize(), _globalSettings, highscoreManager);
        }

        private void RegisterSentry()
        {
            var sentryDsn = _runParameters.SentryEndpoint;
            if (string.IsNullOrEmpty(sentryDsn)) return;
            SentrySdk.Init(o =>
            {
                o.Dsn = sentryDsn;
                o.Release = Constants.BotVersion();
                o.Environment = "Production";
                o.TracesSampleRate = 1.0;
            });
            _logger.Information(LogSource.Geekbot, $"Command Errors will be logged to Sentry: {sentryDsn}");
        }
    }
}