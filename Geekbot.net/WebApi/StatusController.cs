using Nancy;
using Geekbot.net.Lib;

namespace Geekbot.net.WebApi
{
    public class StatusController : NancyModule
    {
        public StatusController()
        {
            Get("/", args =>
            {
                var responseBody = new ApiStatusDto()
                {
                    GeekbotVersion = Constants.BotVersion.ToString(),
                    ApiVersion = Constants.ApiVersion.ToString(),
                    Status = "Online"
                };
                return Response.AsJson(responseBody);
            });
        }
    }
    
    public class ApiStatusDto
    {
        public string GeekbotVersion { get; set; }
        public string ApiVersion { get; set; }
        public string Status { get; set; }
    }
}