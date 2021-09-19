using System.Collections.Generic;
using System.Text.Json.Serialization;
using Geekbot.Core.Interactions.Resolved;

namespace Geekbot.Core.Interactions.Request
{
    public class InteractionResolvedData
    {
        [JsonPropertyName("users")]
        public Dictionary<string, User> Users { get; set; }
        
        [JsonPropertyName("members")]
        public Dictionary<string, Member> Members { get; set; }
        
        [JsonPropertyName("roles")]
        public Dictionary<string, Role> Roles { get; set; }
        
        [JsonPropertyName("channels")]
        public Dictionary<string, Channel> Channels { get; set; }
        
        [JsonPropertyName("messages")]
        public Dictionary<string, Message> Messages { get; set; }
    }
}