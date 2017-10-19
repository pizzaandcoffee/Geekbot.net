using Microsoft.AspNetCore.Mvc;

namespace Geekbot.net.WebApi
{
    [Route("v1/guild")]
    public class GuildProfileControler : Controller
    {
        // Add Guild Info here

        [HttpGet("{id}")]
        public string getGuildInfo(ulong id)
        {
            return id.ToString();
        }
    }
}