using System;
using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.Resolved
{
    public record ThreadMetadata
    {
        [JsonPropertyName("archived")]
        public bool Archived { get; set; }
        
        [JsonPropertyName("auto_archive_duration")]
        public int AutoArchiveDuration { get; set; }
        
        [JsonPropertyName("archive_timestamp")]
        public DateTime ArchiveTimestamp { get; set; }
        
        [JsonPropertyName("locked")]
        public bool Locked { get; set; }
        
        [JsonPropertyName("invitable")]
        public bool Invitable { get; set; }
    }
}