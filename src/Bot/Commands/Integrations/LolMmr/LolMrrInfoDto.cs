using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Integrations.LolMmr
{
    public class LolMrrInfoDto
    {
        [JsonPropertyName("avg")]
        public decimal? Avg { get; set; }
    }
}