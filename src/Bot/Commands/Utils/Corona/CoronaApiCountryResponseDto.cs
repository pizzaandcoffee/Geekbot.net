using Newtonsoft.Json;

namespace Geekbot.Bot.Commands.Utils.Corona
{
    public record CoronaApiCountryResponseDto
    {
        [JsonProperty("country")]
        public string Country { get; init; }

        [JsonProperty("cases")]
        public decimal Cases { get; init; }

        [JsonProperty("deaths")]
        public decimal Deaths { get; init; }

        [JsonProperty("recovered")]
        public decimal Recovered { get; init; }
    }
}