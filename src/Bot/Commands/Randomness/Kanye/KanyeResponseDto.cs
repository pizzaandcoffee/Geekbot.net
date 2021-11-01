using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Randomness.Kanye
{
    public class KanyeResponseDto
    {
        [JsonPropertyName("quote")]
        public string Quote { get; set; }
    }
}