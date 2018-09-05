using System.Linq;
using Discord.Commands;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.net.WebApi.Controllers.Commands
{
    [EnableCors("AllowSpecificOrigin")]
    public class CommandController : Controller
    {
        private readonly CommandService _commands;

        public CommandController(CommandService commands)
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
                        Default = cmdParam.DefaultValue?.ToString(),
                        Type = cmdParam.Type?.ToString()
                    })
                    .ToList()
                let param = string.Join(", !", cmd.Aliases)
                select new CommandDto
                {
                    Name = cmd.Name,
                    Summary = cmd.Summary,
                    IsAdminCommand = param.Contains("admin") || param.Contains("owner"),
                    Aliases = cmd.Aliases.ToList(),
                    Params = cmdParamsObj
                }).ToList();
            return Ok(commandList.FindAll(e => !e.Aliases[0].StartsWith("owner")));
        }
    }
}