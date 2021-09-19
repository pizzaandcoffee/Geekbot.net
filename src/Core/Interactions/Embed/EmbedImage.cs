using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.Embed
{
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-image-structure" />
    public record EmbedImage
    {
        /// <summary>
        /// source url of image (only supports http(s) and attachments)
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// a proxied url of the image
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; set; }
        
        /// <summary>
        /// height of image
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }
        
        /// <summary>
        /// width of image
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}