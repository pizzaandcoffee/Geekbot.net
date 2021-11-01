using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Randomness.Dog
{
    internal class DogResponseDto
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}