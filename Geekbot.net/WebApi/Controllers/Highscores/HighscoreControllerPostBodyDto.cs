using System.ComponentModel.DataAnnotations;
using Geekbot.net.Lib.Highscores;

namespace Geekbot.net.WebApi.Controllers.Highscores
{
    public class HighscoreControllerPostBodyDto
    {
        [Required]
        public ulong GuildId { get; set; }
        
        public HighscoreTypes Type { get; } = HighscoreTypes.messages;
        
        [Range(1, 150)]
        public int Amount { get; } = 50;
    }
}