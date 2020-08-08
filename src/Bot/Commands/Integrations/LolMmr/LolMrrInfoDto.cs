using Newtonsoft.Json;

namespace Geekbot.Bot.Commands.Integrations.LolMmr
{
    public class LolMrrInfoDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal Avg { get; set; } = 0;
    }
}