using System.Text.Json.Serialization;

namespace Geekbot.Interactions.Resolved
{
    public record Channel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("type")]
        public ChannelType Type { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; }
        
        [JsonPropertyName("thread_metadata")]
        public ThreadMetadata ThreadMetadata { get; set; }
        
        [JsonPropertyName("permissions")]
        public string Permissions { get; set; }
    }
}