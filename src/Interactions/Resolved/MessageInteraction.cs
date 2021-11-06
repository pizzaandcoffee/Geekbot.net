using System.Text.Json.Serialization;
using Geekbot.Interactions.Request;

namespace Geekbot.Interactions.Resolved
{
    public record MessageInteraction
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("type")]
        public InteractionType Type { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("user")]
        public User User { get; set; }
    }
}