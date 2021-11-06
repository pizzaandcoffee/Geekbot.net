using System.Globalization;
using Geekbot.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.Web.Controllers.Status;

[ApiController]
[EnableCors("AllowSpecificOrigin")]
public class StatusController : ControllerBase
{
    [Route("/")]
    public IActionResult GetCommands()
    {
        var responseBody = new ApiStatus
        {
            GeekbotVersion = Constants.BotVersion(),
            ApiVersion = Constants.ApiVersion.ToString(CultureInfo.InvariantCulture),
            Status = "Online"
        };
        return Ok(responseBody);
    }
}
