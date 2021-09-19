using System.Collections.Generic;
using System.Text.Json.Serialization;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.MessageComponents;

namespace Geekbot.Core.Interactions.Request
{
    /// <see href="https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-interaction-data-structure" />
    public class InteractionData
    {
        /// <summary>
        /// the ID of the invoked command
        /// </summary>
        /// <remarks>
        /// For: Application Command
        /// </remarks>
        [JsonPropertyName("id")]
        public string Id { get; init; }
        
        /// <summary>
        /// the name of the invoked command
        /// </summary>
        /// <remarks>
        /// For: Application Command
        /// </remarks>
        [JsonPropertyName("name")]
        public string Name { get; init; }
        
        /// <summary>
        /// the type of the invoked command
        /// </summary>
        /// <remarks>
        /// For: Application Command
        /// </remarks>
        [JsonPropertyName("type")]
        public CommandType Type { get; init; }
        
        /// <summary>
        /// converted users + roles + channels
        /// </summary>
        /// <remarks>
        /// For: Application Command
        /// </remarks>
        [JsonPropertyName("resolved")]
        public InteractionResolvedData Resolved { get; init; }
        
        /// <summary>
        /// of application command interaction data option	the params + values from the user
        /// </summary>
        /// <remarks>
        /// For: Application Command
        /// </remarks>
        [JsonPropertyName("options")]
        public List<InteractionOption> Options { get; init; }
        
        /// <summary>
        /// the custom_id of the component
        /// </summary>
        /// <remarks>
        /// For: Component
        /// </remarks>
        [JsonPropertyName("custom_id")]
        public string CustomId { get; init; }
        
        /// <summary>
        /// the type of the component
        /// </summary>
        /// <remarks>
        /// For: Component
        /// </remarks>
        [JsonPropertyName("component_type")]
        public ComponentType ComponentType { get; init; }
        
        /// <summary>
        /// of select option values	the values the user selected
        /// </summary>
        /// <remarks>
        /// Component (Select)
        /// </remarks>
        [JsonPropertyName("values")]
        public string Values { get; init; }
        
        /// <summary>
        /// id the of user or message targetted by a user or message command
        /// </summary>
        /// <remarks>
        /// For: Application Command
        /// Present in: User Command, Message Command
        /// </remarks>
        [JsonPropertyName("target_id")]
        public string TargetId { get; init; }

        public string GetTargetNickname()
        {
            return GetUserNickename(TargetId);
        }

        public string GetUserNickename(string userId)
        {
            var username = Resolved.Members[userId].Nick;
            if (string.IsNullOrEmpty(username))
            {
                username = Resolved.Users[userId].Username;
            }

            return username;
        }
    }
}