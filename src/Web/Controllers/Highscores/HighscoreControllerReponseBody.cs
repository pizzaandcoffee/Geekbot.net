using Geekbot.Core.Highscores;

namespace Geekbot.Web.Controllers.Highscores
{
    public class HighscoreControllerReponseBody
    {
        public int rank { get; set; }
        public HighscoreUserDto user { get; set; }
        public int count { get; set; }
    }
}