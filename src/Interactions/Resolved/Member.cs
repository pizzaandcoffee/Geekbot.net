using System.Text.Json.Serialization;

namespace Geekbot.Interactions.Resolved
{
    public record Member
    {
        [JsonPropertyName("nick")]
        public string Nick { get; set; }
        
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }
        
        [JsonPropertyName("joined_at")]
        public DateTime JoinedAt { get; set; }
        
        [JsonPropertyName("premium_since")]
        public DateTime? PremiumSince { get; set; }
        
        [JsonPropertyName("pending")]
        public bool Pending { get; set; }
        
        [JsonPropertyName("permissions")]
        public string Permissions { get; set; }
        
        [JsonPropertyName("user")]
        public User User { get; set; }
    }
}