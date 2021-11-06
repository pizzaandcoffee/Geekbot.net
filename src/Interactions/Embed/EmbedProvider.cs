using System.Text.Json.Serialization;

namespace Geekbot.Interactions.Embed
{
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-provider-structure" />
    public record EmbedProvider
    {
        /// <summary>
        /// name of provider
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// url of provider
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}