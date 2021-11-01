using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Integrations.LolMmr
{
    public class LolMmrDto
    {
        [JsonPropertyName("ranked")]
        public LolMrrInfoDto Ranked { get; set; }
        
        [JsonPropertyName("normal")]
        public LolMrrInfoDto Normal { get; set; }
        
        [JsonPropertyName("aram")]
        public LolMrrInfoDto ARAM { get; set; }
    }
}