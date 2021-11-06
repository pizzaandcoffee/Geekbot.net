using System.Text.Json.Serialization;

namespace Geekbot.Interactions.MessageComponents
{
    /// <remarks>
    /// Partial emoji with just id, name, and animated
    /// </remarks>
    /// <see href="https://discord.com/developers/docs/resources/emoji#emoji-object" />
    public record ComponentEmoji
    {
        /// <summary>
        /// emoji name
        /// </summary>
        /// <remarks>
        /// can be null only in reaction emoji objects
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// emoji id
        /// </summary>
        /// <see href="https://discord.com/developers/docs/reference#image-formatting" />
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        /// <summary>
        /// whether this emoji can be used, may be false due to loss of Server Boosts
        /// </summary>
        [JsonPropertyName("animated")]
        public bool Animated { get; set; }
    }
}