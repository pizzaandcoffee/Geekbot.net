using System.ComponentModel.DataAnnotations;
using Geekbot.Core.Highscores;

namespace Geekbot.Web.Controllers.Highscores
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