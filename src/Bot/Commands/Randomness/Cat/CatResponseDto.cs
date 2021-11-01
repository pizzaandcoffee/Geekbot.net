using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Randomness.Cat
{
    internal class CatResponseDto
    {
        [JsonPropertyName("file")]
        public string File { get; set; }
    }
}