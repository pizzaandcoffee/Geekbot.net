using System.Text.Json.Serialization;

namespace Geekbot.Interactions.Embed
{
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-footer-structure" />
    public record EmbedFooter
    {
        
        /// <summary>
        /// footer text
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }
        
        /// <summary>
        /// url of footer icon (only supports http(s) and attachments)
        /// </summary>
        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }
        
        /// <summary>
        /// a proxied url of footer icon
        /// </summary>
        [JsonPropertyName("proxy_icon_url")]
        public string ProxyIconUrl { get; set; }
    }
}