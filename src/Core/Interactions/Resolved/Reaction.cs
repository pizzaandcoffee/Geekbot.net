using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.Resolved
{
    public struct Reaction
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        
        [JsonPropertyName("me")]
        public bool Me { get; set; }
        
        [JsonPropertyName("emoji")]
        public Emoji emoji { get; set; }
    }
}