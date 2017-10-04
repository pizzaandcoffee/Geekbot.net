using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using StackExchange.Redis;

namespace Geekbot.net.WebApi
{
    public class WebConfig : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            var services = Program.servicesProvider;
            var redis = services.GetService(typeof(IDatabase)) as IDatabase;
            Console.WriteLine(redis.StringGet("discordToken"));

            //CORS Enable
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

            });
        }
    }
}