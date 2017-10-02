using Nancy;

namespace Geekbot.net.WebApi
{
    public class Status : NancyModule
    {
        public Status()
        {
            Get("/", args =>
            {
                var responseBody = new ApiStatusDto()
                {
                    GeekbotVersion = "3.4",
                    ApiVersion = "0.1",
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