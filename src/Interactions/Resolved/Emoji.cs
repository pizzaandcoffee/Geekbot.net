using System.Text.Json.Serialization;

namespace Geekbot.Interactions.Resolved
{
    public record Emoji
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }
        
        [JsonPropertyName("user")]
        public User User { get; set; }
        
        [JsonPropertyName("require_colons")]
        public bool RequireColons { get; set; }
        
        [JsonPropertyName("managed")]
        public bool Managed { get; set; }
        
        [JsonPropertyName("animated")]
        public bool Animated { get; set; }
        
        [JsonPropertyName("available")]
        public bool Available { get; set; }
    }
}