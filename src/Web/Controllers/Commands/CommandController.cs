using Discord.Commands;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.Web.Controllers.Commands;

[ApiController]
[EnableCors("AllowSpecificOrigin")]
public class CommandController : ControllerBase
{
    private readonly CommandService _commands;

    public CommandController(CommandService commands)
    {
        _commands = commands;
    }

    [HttpGet("/v1/commands")]
    public IActionResult GetCommands()
    {
        var commandList = (from cmd in _commands.Commands
            let cmdParamsObj = cmd.Parameters.Select(cmdParam => new ResponseCommandParam
                {
                    Summary = cmdParam.Summary,
                    Default = cmdParam.DefaultValue?.ToString(),
                    Type = cmdParam.Type?.ToString()
                })
                .ToList()
            let param = string.Join(", !", cmd.Aliases)
            select new ResponseCommand
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