using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Randomness.Greetings
{
    public class GreetingDto
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
        
        [JsonPropertyName("dialect")]
        public string Dialect { get; set; }
        
        [JsonPropertyName("romanization")]
        public string Romanization { get; set; }
        
        [JsonPropertyName("use")]
        public string[] Use { get; set; }
    }
}