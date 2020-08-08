using System.Globalization;
using Geekbot.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.Web.Controllers.Status
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