using AngleSharp.Network.Default;
using Geekbot.net.Lib;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.net.WebApi
{
    [Route("/")]
    public class StatusController : Controller
    {
        [HttpGet()]
        public IActionResult getApiStatus()
        {
            var responseBody = new ApiStatusDto()
            {
                GeekbotVersion = Constants.BotVersion.ToString(),
                ApiVersion = Constants.ApiVersion.ToString(),
                Status = "Online"
            };
            return Ok(responseBody);
        }
    }
    
    public class ApiStatusDto
    {
        public string GeekbotVersion { get; set; }
        public string ApiVersion { get; set; }
        public string Status { get; set; }
    }
}