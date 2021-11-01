using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Randomness.Chuck
{
    internal class ChuckNorrisJokeResponseDto
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}