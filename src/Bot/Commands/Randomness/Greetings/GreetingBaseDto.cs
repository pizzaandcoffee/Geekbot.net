using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Randomness.Greetings
{
    public class GreetingBaseDto
    {
        [JsonPropertyName("language")]
        public string Language { get; set; }
        
        [JsonPropertyName("languageNative")]
        public string LanguageNative { get; set; }
        
        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; }
        
        [JsonPropertyName("script")]
        public string Script { get; set; }

        [JsonPropertyName("primary")]
        public GreetingDto Primary { get; set; }
    }
}