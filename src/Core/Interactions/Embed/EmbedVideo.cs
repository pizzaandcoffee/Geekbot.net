using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.Embed
{
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-video-structure" />
    public record EmbedVideo
    {
        /// <summary>
        /// source url of video
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// a proxied url of the video
        /// </summary>
        [JsonPropertyName("proxy_url")]
        public string ProxyUrl { get; set; }
        
        /// <summary>
        /// height of video
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }
        
        /// <summary>
        /// width of video
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}