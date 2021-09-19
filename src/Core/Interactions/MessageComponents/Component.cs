using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.MessageComponents
{
    /// <see href="https://discord.com/developers/docs/interactions/message-components#component-object-component-structure"/>
    public record Component
    {
        /// <summary>
        /// component type
        /// </summary>
        [JsonPropertyName("type")]
        public ComponentType Type { get; set; }
        
        /// <summary>
        /// a developer-defined identifier for the component, max 100 characters
        /// </summary>
        /// <remarks>
        /// For: Buttons, Select Menus
        /// </remarks>
        [JsonPropertyName("custom_id")]
        public string CustomId { get; set; }
        
        /// <summary>
        /// whether the component is disabled, default false
        /// </summary>
        /// <remarks>
        /// For: Buttons, Select Menus
        /// </remarks>
        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }
        
        /// <summary>
        /// one of button styles
        /// </summary>
        /// <remarks>
        /// For: Buttons
        /// </remarks>
        [JsonPropertyName("style")]
        public ButtonStyle Style { get; set; }
        
        /// <summary>
        /// text that appears on the button, max 80 characters
        /// </summary>
        /// <remarks>
        /// For: Buttons
        /// </remarks>
        [JsonPropertyName("label")]
        public string Label { get; set; }
        
        /// <summary>
        /// name, id, and animated
        /// </summary>
        /// <remarks>
        /// For: Buttons
        /// </remarks>
        [JsonPropertyName("emoji")]
        public ComponentEmoji Emoji { get; set; }
        
        /// <summary>
        /// a url for link-style buttons
        /// </summary>
        /// <remarks>
        /// For: Buttons
        /// </remarks>
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// the choices in the select, max 25
        /// </summary>
        /// <remarks>
        /// For: Select Menus
        /// </remarks>
        [JsonPropertyName("options")]
        public List<SelectOption> Options { get; set; }
        
        /// <summary>
        /// custom placeholder text if nothing is selected, max 100 characters
        /// </summary>
        /// <remarks>
        /// For: Select Menus
        /// </remarks>
        [JsonPropertyName("placeholder")]
        public string Placeholder { get; set; }
        
        /// <summary>
        /// the minimum number of items that must be chosen; default 1, min 0, max 25
        /// </summary>
        /// <remarks>
        /// For: Select Menus
        /// </remarks>
        [JsonPropertyName("min_values")]
        public int MinValues { get; set; }
        
        /// <summary>
        /// the maximum number of items that can be chosen; default 1, max 25
        /// </summary>
        /// <remarks>
        /// For: Select Menus
        /// </remarks>
        [JsonPropertyName("max_values")]
        public int MaxValues { get; set; }
        
        /// <summary>
        /// a list of child components
        /// </summary>
        [JsonPropertyName("components")]
        public List<Component> Components { get; set; }
    }
}