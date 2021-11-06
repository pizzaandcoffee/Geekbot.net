using System.Text.Json.Serialization;

namespace Geekbot.Interactions.ApplicationCommand
{
    /// <remarks>
    /// Required options must be listed before optional options
    /// </remarks>
    /// <see href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-structure"/>
    public record Option
    {
        /// <summary>
        /// the type of option
        /// </summary>
        [JsonPropertyName("type")]
        public OptionType Type { get; set; }
        
        /// <summary>
        /// 1-32 character name
        /// </summary>
        /// <remarks>
        /// Must match ^[\w-]{1,32}$
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// 1-100 character description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        /// <summary>
        ///	if the parameter is required or optional--default false
        /// </summary>
        [JsonPropertyName("required")]
        public bool Required { get; set; }
        
        /// <summary>
        /// choices for STRING, INTEGER, and NUMBER types for the user to pick from, max 25
        /// </summary>
        [JsonPropertyName("choices")]
        public List<OptionChoice> Choices { get; set; }
        
        /// <summary>
        /// if the option is a subcommand or subcommand group type, this nested options will be the parameters
        /// </summary>
        [JsonPropertyName("options")]
        public List<Option> Options { get; set; }
    }
}