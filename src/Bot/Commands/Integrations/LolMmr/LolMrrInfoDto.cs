using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Integrations.LolMmr
{
    public class LolMrrInfoDto
    {
        [JsonPropertyName("avg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal Avg { get; set; } = 0;
    }
}