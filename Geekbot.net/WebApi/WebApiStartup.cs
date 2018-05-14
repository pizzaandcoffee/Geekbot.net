using System;
using System.Net;
using System.Reflection;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Logger;
using Geekbot.net.WebApi.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Geekbot.net.WebApi
{
    public class WebApiStartup
    {
        public static void StartWebApi(IGeekbotLogger logger, RunParameters runParameters, CommandService commandService)
        {
            WebHost.CreateDefaultBuilder()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, int.Parse(runParameters.ApiPort));
                })
                .ConfigureServices(services =>
                {
                    services.AddMvc();
                    services.AddSingleton<CommandService>(commandService);
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
                .UseSetting(WebHostDefaults.ApplicationKey, typeof(Program).GetTypeInfo().Assembly.FullName)
                .Build().Run();
        }
    }
}