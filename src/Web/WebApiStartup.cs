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
                    services.AddMvc();
                    services.AddSingleton(commandService);
                    services.AddSingleton(databaseContext);
                    services.AddSingleton(client);
                    services.AddSingleton(globalSettings);
                    services.AddSingleton(highscoreManager);
                    services.AddCors(options =>
                    {
                        options.AddPolicy("AllowSpecificOrigin",
                            builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
                    });
                })
                .Configure(app =>
                {
                    app.UseMvc();
                    app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build());
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