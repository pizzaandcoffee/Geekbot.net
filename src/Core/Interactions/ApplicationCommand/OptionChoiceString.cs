using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.ApplicationCommand
{
    /// <see href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-choice-structure"/>
    public record OptionChoiceString : OptionChoice
    {
        /// <summary>
        /// value of the choice, up to 100 characters if string
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}