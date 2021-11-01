using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Utils.Corona
{
    public record CoronaApiCountryResponseDto
    {
        [JsonPropertyName("country")]
        public string Country { get; init; }

        [JsonPropertyName("cases")]
        public decimal Cases { get; init; }

        [JsonPropertyName("deaths")]
        public decimal Deaths { get; init; }

        [JsonPropertyName("recovered")]
        public decimal Recovered { get; init; }
    }
}