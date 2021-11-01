using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Randomness.Dad
{
    internal class DadJokeResponseDto
    {
        [JsonPropertyName("joke")]
        public string Joke { get; set; }
    }
}