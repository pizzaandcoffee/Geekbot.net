using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Database;
using Geekbot.net.Handlers;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Clients;
using Geekbot.net.Lib.Converters;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.GlobalSettings;
using Geekbot.net.Lib.Highscores;
using Geekbot.net.Lib.KvInMemoryStore;
using Geekbot.net.Lib.Levels;
using Geekbot.net.Lib.Localization;
using Geekbot.net.Lib.Logger;
using Geekbot.net.Lib.Media;
using Geekbot.net.Lib.RandomNumberGenerator;
using Geekbot.net.Lib.ReactionListener;
using Geekbot.net.Lib.UserRepository;
using Geekbot.net.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WikipediaApi;

namespace Geekbot.net
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
            
            logger.Information(LogSource.Geekbot, "Connecting to Database");
            var database = ConnectToDatabase();
            _globalSettings = new GlobalSettings(database);

            logger.Information(LogSource.Geekbot, "Connecting to Discord");
            SetupDiscordClient();
            await Login();
            _logger.Information(LogSource.Geekbot, $"Now Connected as {_client.CurrentUser.Username} to {_client.Guilds.Count} Servers");
            await _client.SetGameAsync(_globalSettings.GetKey("Game"));

            _logger.Information(LogSource.Geekbot, "Loading Dependencies and Handlers");
            RegisterDependencies();
            await RegisterHandlers();

            _logger.Information(LogSource.Api, "Starting Web API");
            StartWebApi();
            
            _logger.Information(LogSource.Geekbot, "Done and ready for use");

            await Task.Delay(-1);
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
            var fortunes = new FortunesProvider(_logger);
            var mediaProvider = new MediaProvider(_logger);
            var malClient = new MalClient(_globalSettings, _logger);
            var levelCalc = new LevelCalc();
            var emojiConverter = new EmojiConverter();
            var mtgManaConverter = new MtgManaConverter();
            var wikipediaClient = new WikipediaClient();
            var randomNumberGenerator = new RandomNumberGenerator();
            var kvMemoryStore = new KvInInMemoryStore();
            var translationHandler = new TranslationHandler(_databaseInitializer.Initialize(), _logger);
            var errorHandler = new ErrorHandler(_logger, translationHandler, _runParameters.ExposeErrors);
            
            services.AddSingleton(_userRepository);
            services.AddSingleton<IGeekbotLogger>(_logger);
            services.AddSingleton<ILevelCalc>(levelCalc);
            services.AddSingleton<IEmojiConverter>(emojiConverter);
            services.AddSingleton<IFortunesProvider>(fortunes);
            services.AddSingleton<IMediaProvider>(mediaProvider);
            services.AddSingleton<IMalClient>(malClient);
            services.AddSingleton<IMtgManaConverter>(mtgManaConverter);
            services.AddSingleton<IWikipediaClient>(wikipediaClient);
            services.AddSingleton<IRandomNumberGenerator>(randomNumberGenerator);
            services.AddSingleton<IKvInMemoryStore>(kvMemoryStore);
            services.AddSingleton<IGlobalSettings>(_globalSettings);
            services.AddSingleton<IErrorHandler>(errorHandler);
            services.AddSingleton<ITranslationHandler>(translationHandler);
            services.AddSingleton<IReactionListener>(_reactionListener);
            services.AddSingleton(_client);
            services.AddTransient<IHighscoreManager>(e => new HighscoreManager(_databaseInitializer.Initialize(), _userRepository));
            services.AddTransient(e => _databaseInitializer.Initialize());
            
            _servicesProvider = services.BuildServiceProvider();
        }
        
        private async Task RegisterHandlers()
        {
            var applicationInfo = await _client.GetApplicationInfoAsync();
            
            _commands = new CommandService();
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _servicesProvider);
            
            var commandHandler = new CommandHandler(_databaseInitializer.Initialize(), _client, _logger, _servicesProvider, _commands, applicationInfo);
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
            if (_runParameters.DisableApi)
            {
                _logger.Warning(LogSource.Api, "Web API is disabled");
                return;
            }
            
            var highscoreManager = new HighscoreManager(_databaseInitializer.Initialize(), _userRepository);
            WebApiStartup.StartWebApi(_logger, _runParameters, _commands, _databaseInitializer.Initialize(), _client, _globalSettings, highscoreManager);
        }
    }
}