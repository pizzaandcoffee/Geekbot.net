using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Geekbot.Core.Interactions.ApplicationCommand;

namespace Geekbot.Core.Interactions.Request
{
    /// <see href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-interaction-data-option-structure" />
    public record InteractionOption
    {
        /// <summary>
        /// the name of the parameter
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// value of application command option type
        /// </summary>
        [JsonPropertyName("type")]
        public OptionType Type { get; set; }
        
        /// <summary>
        /// the value of the pair
        /// </summary>
        [JsonPropertyName("value")]
        public JsonElement Value { get; set; }
        
        /// <summary>
        /// present if this option is a group or subcommand
        /// </summary>
        [JsonPropertyName("options")]
        public List<InteractionOption> Options { get; set; }
    }
}