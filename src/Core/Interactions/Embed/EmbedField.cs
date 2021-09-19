using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.Embed
{
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-field-structure" />
    public record EmbedField
    {
        /// <summary>
        /// name of the field
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// value of the field
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }
        
        /// <summary>
        /// whether or not this field should display inline
        /// </summary>
        [JsonPropertyName("inline")]
        public bool Inline { get; set; }
    }
}