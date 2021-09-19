using System.Net;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.Core;
using Geekbot.Core.Database;
using Geekbot.Core.GlobalSettings;
using Geekbot.Core.Highscores;
using Geekbot.Core.Logger;
using Geekbot.Web.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Geekbot.Web
{
    public static class WebApiStartup
    {
        // Using the "Microsoft.NET.Sdk.Web" SDK requires a static main function...
        public static void Main() {}
        
        public static void StartWebApi(IGeekbotLogger logger, RunParameters runParameters, CommandService commandService,
            DatabaseContext databaseContext, DiscordSocketClient client, IGlobalSettings globalSettings, IHighscoreManager highscoreManager)
        {
            WebHost.CreateDefaultBuilder()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, int.Parse(runParameters.ApiPort));
                })
                .ConfigureServices(services =>
                {
                    services.AddControllers().AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    });
                    services.AddCors(options =>
                    {
                        options.AddPolicy("AllowSpecificOrigin",
                            builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
                    });
                    services.AddSentry();
                    
                    services.AddSingleton(databaseContext);
                    services.AddSingleton(globalSettings);
                    services.AddSingleton(highscoreManager);

                    if (runParameters.DisableGateway) return;
                    services.AddSingleton(commandService);
                    services.AddSingleton(client);
                })
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build());
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddProvider(new AspLogProvider(logger));
                })
                .UseSetting(WebHostDefaults.ApplicationKey, typeof(WebApiStartup).GetTypeInfo().Assembly.FullName)
                .Build().Run();
        }
    }
}