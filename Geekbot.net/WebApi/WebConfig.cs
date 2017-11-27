using System.Diagnostics;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace Geekbot.net.WebApi
{
    public class WebConfig : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {

            //CORS Enable
            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type")
                    .WithHeader("Last-Modified", Process.GetCurrentProcess().StartTime.ToString());
            });
        }
    }
}