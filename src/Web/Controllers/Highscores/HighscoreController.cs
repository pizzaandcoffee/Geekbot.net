using Geekbot.Core.Highscores;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.Web.Controllers.Highscores;

[ApiController]
[EnableCors("AllowSpecificOrigin")]
public class HighscoreController : ControllerBase
{
    private readonly IHighscoreManager _highscoreManager;

    public HighscoreController(IHighscoreManager highscoreManager)
    {
        _highscoreManager = highscoreManager;
    }

    [HttpPost]
    [Route("/v1/highscore")]
    public IActionResult GetHighscores([FromBody] HighscoreControllerPostBody body)
    {
        if (!ModelState.IsValid || body == null)
        {
            var error = new SerializableError(ModelState);
            return BadRequest(error);
        }

        Dictionary<HighscoreUserDto, int> list;
        try
        {
            list = _highscoreManager.GetHighscoresWithUserData(body.Type, body.GuildId, body.Amount);
        }
        catch (HighscoreListEmptyException)
        {
            return NotFound(new ApiError
            {
                Message = $"No {body.Type} found on this server"
            });
        }

        var response = new List<HighscoreControllerReponseBody>();
        var counter = 1;
        foreach (var item in list)
        {
            response.Add(new HighscoreControllerReponseBody
            {
                count = item.Value,
                rank = counter,
                user = item.Key
            });
            counter++;
        }

        return Ok(response);
    }
}