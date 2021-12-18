using System.Text;
using CommandLine;
using Geekbot.Bot;
using Geekbot.Core;
using Geekbot.Core.BotCommandLookup;
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

//
// Parse Parameters
//
RunParameters runParameters = null!;
Parser.Default.ParseArguments<RunParameters>(args)
    .WithParsed(e => runParameters = e)
    .WithNotParsed(_ => Environment.Exit(GeekbotExitCode.InvalidArguments.GetHashCode()));

//
// Print Logo
//
var logo = new StringBuilder();
logo.AppendLine(@"  ____ _____ _____ _  ______   ___ _____");
logo.AppendLine(@" / ___| ____| ____| |/ / __ ) / _ \\_  _|");
logo.AppendLine(@"| |  _|  _| |  _| | ' /|  _ \| | | || |");
logo.AppendLine(@"| |_| | |___| |___| . \| |_) | |_| || |");
logo.AppendLine(@" \____|_____|_____|_|\_\____/ \___/ |_|");
logo.AppendLine($"Version {Constants.BotVersion()} ".PadRight(41, '='));
Console.WriteLine(logo.ToString());

//
// Init Logger
//
var logger = new GeekbotLogger(runParameters);
logger.Information(LogSource.Geekbot, "Starting...");

//
// Connect to Database
//
logger.Information(LogSource.Geekbot, "Connecting to Database");
var databaseInitializer = new DatabaseInitializer(runParameters, logger);
var database = databaseInitializer.Initialize();
database.Database.EnsureCreated();
if (!runParameters.InMemory)
{
    logger.Information(LogSource.Geekbot, "Using Postgres");
    database.Database.Migrate();
}
else
{
    logger.Information(LogSource.Geekbot, "Using In-Memory Database");
}

var globalSettings = new GlobalSettings(database);

//
// Register Services
//
logger.Information(LogSource.Geekbot, "Registering Services");
RegisterSentry();
var serviceProvider = RegisterServices();

//
// Start Gateway Bot
//
if (!runParameters.DisableGateway)
{
    new BotStartup(serviceProvider, logger, runParameters, globalSettings).Start();
}
else
{
    logger.Information(LogSource.Geekbot, "Gateway disabled");
}

//
// Start WebApi
//
if (!runParameters.DisableApi)
{
    var botCommands = new CommandLookup(typeof(BotStartup).Assembly).GetCommands();
    WebApiStartup.StartWebApi(serviceProvider.BuildServiceProvider(), logger, runParameters, databaseInitializer.Initialize(), globalSettings, botCommands);
}
else
{
    logger.Information(LogSource.Geekbot, "Web API disabled");
}

ServiceCollection RegisterServices()
{
    var services = new ServiceCollection();
    var userRepository = new UserRepository(databaseInitializer.Initialize(), logger);
    var reactionListener = new ReactionListener(databaseInitializer.Initialize());
    var guildSettingsManager = new GuildSettingsManager(databaseInitializer.Initialize());
    var fortunes = new FortunesProvider(logger);
    var levelCalc = new LevelCalc();
    var mtgManaConverter = new MtgManaConverter();
    var wikipediaClient = new WikipediaClient();
    var randomNumberGenerator = new RandomNumberGenerator();
    var mediaProvider = new MediaProvider(logger, randomNumberGenerator);
    var kvMemoryStore = new KvInInMemoryStore();
    var errorHandler = new ErrorHandler(logger, runParameters, () => Geekbot.Core.Localization.Internal.SomethingWentWrong);
    var diceParser = new DiceParser(randomNumberGenerator);
    
    services.AddSingleton<IUserRepository>(userRepository);
    services.AddSingleton<IGeekbotLogger>(logger);
    services.AddSingleton<ILevelCalc>(levelCalc);
    services.AddSingleton<IFortunesProvider>(fortunes);
    services.AddSingleton<IMediaProvider>(mediaProvider);
    services.AddSingleton<IMtgManaConverter>(mtgManaConverter);
    services.AddSingleton<IWikipediaClient>(wikipediaClient);
    services.AddSingleton<IRandomNumberGenerator>(randomNumberGenerator);
    services.AddSingleton<IKvInMemoryStore>(kvMemoryStore);
    services.AddSingleton<IGlobalSettings>(globalSettings);
    services.AddSingleton<IErrorHandler>(errorHandler);
    services.AddSingleton<IDiceParser>(diceParser);
    services.AddSingleton<IReactionListener>(reactionListener);
    services.AddSingleton<IGuildSettingsManager>(guildSettingsManager);
    services.AddTransient<IHighscoreManager>(e => new HighscoreManager(databaseInitializer.Initialize(), userRepository));
    services.AddTransient(e => databaseInitializer.Initialize());

    return services;
}

void RegisterSentry()
{
    var sentryDsn = runParameters.SentryEndpoint;
    if (string.IsNullOrEmpty(sentryDsn)) return;
    SentrySdk.Init(o =>
    {
        o.Dsn = sentryDsn;
        o.Release = Constants.BotVersion();
        o.Environment = "Production";
        o.TracesSampleRate = 1.0;
    });
    logger.Information(LogSource.Geekbot, $"Command Errors will be logged to Sentry: {sentryDsn}");
}