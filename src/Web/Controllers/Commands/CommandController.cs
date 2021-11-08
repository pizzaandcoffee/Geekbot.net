using Geekbot.Core.BotCommandLookup;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.Web.Controllers.Commands;

[ApiController]
[EnableCors("AllowSpecificOrigin")]
public class CommandController : ControllerBase
{
    private readonly List<CommandInfo> _commandInfos;

    public CommandController(List<CommandInfo> commandInfos)
    {
        _commandInfos = commandInfos;
    }

    [HttpGet("/v1/commands")]
    public IActionResult GetCommands()
    {
        var commandList = _commandInfos.Select(cmd => new ResponseCommand()
        {
            Name = cmd.Name,
            Summary = cmd.Summary,
            IsAdminCommand = cmd.Name.StartsWith("admin") || cmd.Name.StartsWith("owner"),
            Aliases = new List<string>() { cmd.Name },
            Params = cmd.Parameters.Select(dict => new ResponseCommandParam()
            {
                Summary = dict.Value.Summary,
                Default = dict.Value.DefaultValue,
                Type = dict.Value.Type
            }).ToList()
        });
        return Ok(commandList);
    }
}