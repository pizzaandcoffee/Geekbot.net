using System.Collections.Generic;
using Geekbot.net.Lib.Highscores;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.net.WebApi.Controllers.Highscores
{
    [EnableCors("AllowSpecificOrigin")]
    public class HighscoreController : Controller
    {
        private readonly IHighscoreManager _highscoreManager;

        public HighscoreController(IHighscoreManager highscoreManager)
        {
            _highscoreManager = highscoreManager;
        }

        [HttpPost]
        [Route("/v1/highscore")]
        public IActionResult GetHighscores([FromBody] HighscoreControllerPostBodyDto body)
        {
            if (!ModelState.IsValid || body == null)
            {
                var error = new SerializableError(ModelState);
                return BadRequest(error);
            }

            SortedDictionary<HighscoreUserDto, int> list;
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
}