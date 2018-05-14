using System.Linq;
using Discord.Commands;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.net.WebApi.Controllers.Commands
{
    [EnableCors("AllowSpecificOrigin")]
    public class HelpController : Controller
    {
        private readonly CommandService _commands;

        public HelpController(CommandService commands)
        {
            _commands = commands;
        }
        
        [Route("/v1/commands")]
        public IActionResult GetCommands()
        {
            var commandList = (from cmd in _commands.Commands
                let cmdParamsObj = cmd.Parameters.Select(cmdParam => new CommandParamDto
                    {
                        Summary = cmdParam.Summary,
                        Default = cmdParam.DefaultValue?.ToString() ?? null,
                        Type = cmdParam.Type?.ToString()
                    })
                    .ToList()
                let param = string.Join(", !", cmd.Aliases)
                select new CommandDto
                {
                    Name = cmd.Name,
                    Summary = cmd.Summary,
                    IsAdminCommand = (param.Contains("admin")),
                    Aliases = cmd.Aliases.ToArray(),
                    Params = cmdParamsObj
                }).ToList();
            return Ok(commandList);
        }
    }
}