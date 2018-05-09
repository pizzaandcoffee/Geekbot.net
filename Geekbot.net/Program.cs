using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Database;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Audio;
using Geekbot.net.Lib.Clients;
using Geekbot.net.Lib.Converters;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Levels;
using Geekbot.net.Lib.Localization;
using Geekbot.net.Lib.Logger;
using Geekbot.net.Lib.Media;
using Geekbot.net.Lib.ReactionListener;
using Geekbot.net.Lib.UserRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Hosting.Self;
using StackExchange.Redis;
using WikipediaApi;

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
        private GeekbotLogger _logger;
        private IUserRepository _userRepository;
        private bool _firstStart;
        private RunParameters _runParameters;

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
            logo.AppendLine("=========================================");
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

            try
            {
                var redisMultiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379");
                _redis = redisMultiplexer.GetDatabase(6);
                logger.Information(LogSource.Redis, $"Connected to db {_redis.Database}");
            }
            catch (Exception e)
            {
                logger.Error(LogSource.Redis, "Redis Connection Failed", e);
                Environment.Exit(GeekbotExitCode.RedisConnectionFailed.GetHashCode());
            }
            
            _token = runParameters.Token ?? _redis.StringGet("discordToken");
            if (_token.IsNullOrEmpty)
            {
                Console.Write("Your bot Token: ");
                var newToken = Console.ReadLine();
                _redis.StringSet("discordToken", newToken);
                _redis.StringSet("Game", "Ping Pong");
                _token = newToken;
                _firstStart = true;
            }

            var database = new DatabaseInitializer(runParameters, logger).Initzialize();

            _services = new ServiceCollection();
            
            _userRepository = new UserRepository(_redis, logger);
            var fortunes = new FortunesProvider(logger);
            var mediaProvider = new MediaProvider(logger);
            var malClient = new MalClient(_redis, logger);
            var levelCalc = new LevelCalc();
            var emojiConverter = new EmojiConverter();
            var mtgManaConverter = new MtgManaConverter();
            var wikipediaClient = new WikipediaClient();
            var audioUtils = new AudioUtils();
            
            _services.AddSingleton<IDatabase>(_redis);
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
            _services.AddSingleton<DatabaseContext>(database);

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
                    await _client.SetGameAsync(_redis.StringGet("Game"));
                    _logger.Information(LogSource.Geekbot, $"Now Connected as {_client.CurrentUser.Username} to {_client.Guilds.Count} Servers");

                    _logger.Information(LogSource.Geekbot, "Registering Stuff");
                    var translationHandler = new TranslationHandler(_client.Guilds, _redis, _logger);
                    var errorHandler = new ErrorHandler(_logger, translationHandler, _runParameters.ExposeErrors);
                    var reactionListener = new ReactionListener(_redis);
                    await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
                    _services.AddSingleton(_commands);
                    _services.AddSingleton<IErrorHandler>(errorHandler);
                    _services.AddSingleton<ITranslationHandler>(translationHandler);
                    _services.AddSingleton(_client);
                    _services.AddSingleton<IReactionListener>(reactionListener);
                    _servicesProvider = _services.BuildServiceProvider();
                    
                    var handlers = new Handlers(_client, _logger, _redis, _servicesProvider, _commands, _userRepository, reactionListener);
                    
                    _client.MessageReceived += handlers.RunCommand;
                    _client.MessageReceived += handlers.UpdateStats;
                    _client.MessageDeleted += handlers.MessageDeleted;
                    _client.UserJoined += handlers.UserJoined;
                    _client.UserUpdated += handlers.UserUpdated;
                    _client.UserLeft += handlers.UserLeft;
                    _client.ReactionAdded += handlers.ReactionAdded;
                    _client.ReactionRemoved += handlers.ReactionRemoved;

                    if (_firstStart || _runParameters.Reset)
                    {
                        _logger.Information(LogSource.Geekbot, "Finishing setup");
                        await FinishSetup();
                        _logger.Information(LogSource.Geekbot, "Setup finished");
                    }
                    if (!_runParameters.DisableApi)
                    {
                        StartWebApi();
                    }
                    
                    _logger.Information(LogSource.Geekbot, "Done and ready for use");
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Could not connect...", e);
                Environment.Exit(GeekbotExitCode.CouldNotLogin.GetHashCode());
            }
        }

        private async Task<bool> IsConnected()
        {
            while (!_client.ConnectionState.Equals(ConnectionState.Connected))
                await Task.Delay(25);
            return true;
        }

        private void StartWebApi()
        {
            _logger.Information(LogSource.Api, "Starting Webserver");
            var webApiUrl = new Uri("http://localhost:12995");
            new NancyHost(webApiUrl).Start();
            _logger.Information(LogSource.Api, $"Webserver now running on {webApiUrl}");
        }

        private async Task<Task> FinishSetup()
        {
            try
            {
                // ToDo: Set bot avatar
                var appInfo = await _client.GetApplicationInfoAsync();
                _redis.StringSet("botOwner", appInfo.Owner.Id);
            }
            catch (Exception e)
            {
                _logger.Warning(LogSource.Geekbot, "Setup Failed, couldn't retrieve discord application data", e);
            }
            return Task.CompletedTask;
        }
    }
}