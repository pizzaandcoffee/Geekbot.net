using System.Text.Json.Serialization;

namespace Geekbot.Interactions.Resolved
{
    public record Attachment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("filename")]
        public string Filename { get; set; }
        
        [JsonPropertyName("content_type")]
        public string ContentType { get; set; }
        
        [JsonPropertyName("size")]
        public int Size { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; set; }
        
        [JsonPropertyName("height")]
        public int Height { get; set; }
        
        [JsonPropertyName("width")]
        public int Width { get; set; }
        
        [JsonPropertyName("ephemeral")]
        public bool Ephemeral { get; set; }
    }
}