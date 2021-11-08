using System.Net;
using System.Reflection;
using Geekbot.Core;
using Geekbot.Core.BotCommandLookup;
using Geekbot.Core.Database;
using Geekbot.Core.GlobalSettings;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.Highscores;
using Geekbot.Core.Logger;
using Geekbot.Core.UserRepository;
using Geekbot.Interactions;
using Geekbot.Web.Logging;

namespace Geekbot.Web;

public static class WebApiStartup
{
    // Using the "Microsoft.NET.Sdk.Web" SDK requires a static main function...
    public static void Main()
    {
    }

    public static void StartWebApi(IServiceProvider commandProvider, IGeekbotLogger logger, RunParameters runParameters, DatabaseContext databaseContext, IGlobalSettings globalSettings, List<CommandInfo> commandInfos)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions() { ApplicationName = typeof(WebApiStartup).GetTypeInfo().Assembly.FullName });
        builder.WebHost.UseKestrel(options => options.Listen(IPAddress.Any, int.Parse(runParameters.ApiPort)));

        builder.Services.AddControllers();
        builder.Services.AddCors(options => options.AddPolicy("AllowSpecificOrigin", cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

        var interactionCommandManager = new InteractionCommandManager(commandProvider, commandProvider.GetService<IGuildSettingsManager>());
        var highscoreManager = new HighscoreManager(commandProvider.GetService<DatabaseContext>(), commandProvider.GetService<IUserRepository>());

        builder.Services.AddSingleton(databaseContext);
        builder.Services.AddSingleton(globalSettings);
        builder.Services.AddSingleton(highscoreManager);
        builder.Services.AddSingleton(logger);
        builder.Services.AddSingleton<IInteractionCommandManager>(interactionCommandManager);
        builder.Services.AddSingleton(commandInfos);

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        builder.Logging.AddProvider(new AspLogProvider(logger));

        var app = builder.Build();
        app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build());
        app.MapControllers();

        app.Run();
    }
}