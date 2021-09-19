using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.MessageComponents
{
    /// <see href="https://discord.com/developers/docs/interactions/message-components#select-menu-object-select-option-structure" />
    public record SelectOption
    {
        /// <summary>
        /// the user-facing name of the option, max 100 characters
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }
        
        /// <summary>
        /// the dev-define value of the option, max 100 characters
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }
        
        /// <summary>
        /// an additional description of the option, max 100 characters
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// id, name, and animated
        /// </summary>
        [JsonPropertyName("emoji")]
        public ComponentEmoji Emoji { get; set; }
        
        /// <summary>
        /// will render this option as selected by default
        /// </summary>
        [JsonPropertyName("default")]
        public bool Default { get; set; }
    }
}