using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.Embed
{
    public record EmbedAuthor
    {
        
        /// <summary>
        /// name of author
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// url of author
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// url of author icon (only supports http(s) and attachments)
        /// </summary>
        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }
        
        /// <summary>
        /// a proxied url of author icon
        /// </summary>
        [JsonPropertyName("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }
}