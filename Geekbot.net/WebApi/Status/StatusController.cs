using Geekbot.net.Lib;
using Nancy;

namespace Geekbot.net.WebApi.Status
{
    public class StatusController : NancyModule
    {
        public StatusController()
        {
            Get("/", args =>
            {
                var responseBody = new ApiStatusDto
                {
                    GeekbotVersion = Constants.BotVersion(),
                    ApiVersion = Constants.ApiVersion.ToString(),
                    Status = "Online"
                };
                return Response.AsJson(responseBody);
            });
        }
    }
}