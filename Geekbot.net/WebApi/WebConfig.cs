using System.Diagnostics;
using Discord.Commands;
using Geekbot.net.Lib.Logger;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Geekbot.net.WebApi
{
    public class WebConfig : DefaultNancyBootstrapper
    {
        private readonly GeekbotLogger _logger;
        private readonly CommandService _commands;

        public WebConfig(GeekbotLogger logger, CommandService commands)
        {
            _logger = logger;
            _commands = commands;
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            // Register Dependencies
            container.Register<IGeekbotLogger>(_logger);
            container.Register<CommandService>(_commands);
            
            // Enable CORS
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx =>
            {
                _logger.Information(LogSource.Api, ctx.Request.Path.ToString());
                
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type")
                    .WithHeader("Last-Modified", Process.GetCurrentProcess().StartTime.ToString());
            });
        }
    }
}