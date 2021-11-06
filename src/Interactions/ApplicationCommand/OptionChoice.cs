using System.Text.Json.Serialization;

namespace Geekbot.Interactions.ApplicationCommand
{
    /// <remarks>
    /// If you specify choices for an option, they are the only valid values for a user to pick
    /// </remarks>
    /// <see href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-choice-structure"/>
    public record OptionChoice
    {
        /// <summary>
        /// 1-100 character choice name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// value of the choice, up to 100 characters if string
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}