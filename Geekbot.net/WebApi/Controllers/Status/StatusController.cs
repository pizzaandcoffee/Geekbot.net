using System.Globalization;
using Geekbot.net.Lib;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.net.WebApi.Controllers.Status
{
    [EnableCors("AllowSpecificOrigin")]
    public class StatusController : Controller
    {
        [Route("/")]
        public IActionResult GetCommands()
        {
            var responseBody = new ApiStatusDto
            {
                GeekbotVersion = Constants.BotVersion(),
                ApiVersion = Constants.ApiVersion.ToString(CultureInfo.InvariantCulture),
                Status = "Online"
            };
            return Ok(responseBody);
        }
    }
}