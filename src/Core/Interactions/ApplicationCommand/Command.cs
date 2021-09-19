using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.ApplicationCommand
{
    /// <see href="https://discord.com/developers/docs/interactions/application-commands#application-command-object"/>
    public record Command
    {
        /// <summary>
        /// unique id of the command
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        /// <summary>
        /// the type of command, defaults 1 if not set
        /// </summary>
        [JsonPropertyName("type")]
        public CommandType Type { get; set; }
        
        /// <summary>
        /// unique id of the parent application
        /// </summary>
        [JsonPropertyName("application_id")]
        public string ApplicationId { get; set; }
        
        /// <summary>
        ///	guild id of the command, if not global
        /// </summary>
        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; }
        
        /// <summary>
        /// 1-32 character name
        /// </summary>
        /// <remarks>
        /// CHAT_INPUT command names and command option names must match the following regex ^[\w-]{1,32}$ with the unicode flag set. If there is a lowercase variant of any letters used, you must use those.
        /// Characters with no lowercase variants and/or uncased letters are still allowed. USER and MESSAGE commands may be mixed case and can include spaces.
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// 1-100 character description for CHAT_INPUT commands, empty string for USER and MESSAGE commands
        /// </summary>
        /// <remarks>
        /// Exclusive: CHAT_INPUT
        /// </remarks>
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// the parameters for the command, max 25
        /// </summary>
        /// <remarks>
        /// Exclusive: CHAT_INPUT
        /// </remarks>
        [JsonPropertyName("options")]
        public List<Option> Options { get; set; }

        /// <summary>
        /// whether the command is enabled by default when the app is added to a guild
        /// </summary>
        [JsonPropertyName("default_permission")]
        public bool DefaultPermission { get; set; } = true;
        
        /// <summary>
        /// autoincrementing version identifier updated during substantial record changes
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
}