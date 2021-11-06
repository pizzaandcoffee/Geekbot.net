using System.Text.RegularExpressions;
using Geekbot.Core;
using Geekbot.Core.GlobalSettings;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.Web.Controllers;

/*
 * Sometimes for some unknown reason command responses may become incredibly slow.
 * Because i don't have time to debug it and may not always know directly when it happens,
 * i want to give some trusted people the possibility to restart the bot.
 * The kill code must be set manually in the db, it should end it `---1'
 *
 * ToDo: Actually fix the underlying issue
 */

[ApiController]
public class KillController : ControllerBase
{
    private readonly IGlobalSettings _globalSettings;
    private const string KillCodeDbKey = "KillCode";

    public KillController(IGlobalSettings globalSettings)
    {
        _globalSettings = globalSettings;
    }

    [HttpGet("/v1/kill/{providedKillCode}")]
    public async Task<IActionResult> KillTheBot([FromRoute] string providedKillCode)
    {
        var killCode = _globalSettings.GetKey(KillCodeDbKey);
        if (providedKillCode != killCode)
        {
            return Unauthorized(new ApiError { Message = $"Go Away ({GetKillCodeInt(killCode)})" });
        }

        await UpdateKillCode(killCode);
#pragma warning disable CS4014
        Kill();
#pragma warning restore CS4014
        return Ok();
    }

    private int GetKillCodeInt(string code)
    {
        var group = Regex.Match(code, @".+(\-\-\-(?<int>\d+))").Groups["int"];
        return group.Success ? int.Parse(group.Value) : 0;
    }

    private async Task UpdateKillCode(string oldCode)
    {
        var newKillCodeInt = new Random().Next(1, 100);
        var newCode = oldCode.Replace($"---{GetKillCodeInt(oldCode)}", $"---{newKillCodeInt}");
        await _globalSettings.SetKey(KillCodeDbKey, newCode);
    }

    private async Task Kill()
    {
        // wait a second so the http response can still be sent successfully 
        await Task.Delay(1000);
        Environment.Exit(GeekbotExitCode.KilledByApiCall.GetHashCode());
    }
}