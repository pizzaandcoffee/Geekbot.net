using System.Text.Json.Serialization;

namespace Geekbot.Interactions.Embed
{
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-thumbnail-structure" />
    public record EmbedThumbnail
    {
        
        /// <summary>
        /// source url of thumbnail (only supports http(s) and attachments)
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// a proxied url of the thumbnail
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; set; }
        
        /// <summary>
        /// height of thumbnail
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }
        
        /// <summary>
        /// width of thumbnail
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}