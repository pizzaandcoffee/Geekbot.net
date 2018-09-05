using Geekbot.net.Lib.Highscores;

namespace Geekbot.net.WebApi.Controllers.Highscores
{
    public class HighscoreControllerReponseBody
    {
        public int rank { get; set; }
        public HighscoreUserDto user { get; set; }
        public int count { get; set; }
    }
}