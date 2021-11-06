using System.Text.Json.Serialization;
using Geekbot.Interactions.Resolved;

namespace Geekbot.Interactions.Request
{
    /// <see href="https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-interaction-structure" />
    public record Interaction
    {
        /// <summary>
        /// id of the interaction
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; }
        
        /// <summary>
        /// id of the application this interaction is for
        /// </summary>
        [JsonPropertyName("application_id")]
        public string ApplicationId { get; init; }
        
        /// <summary>
        /// the type of interaction
        /// </summary>
        [JsonPropertyName("type")]
        public InteractionType Type { get; init; }
        
        /// <summary>
        /// the command data payload
        /// </summary>
        [JsonPropertyName("data")]
        public InteractionData Data { get; init; }
        
        /// <summary>
        /// the guild it was sent from
        /// </summary>
        [JsonPropertyName("guild_id")]
        public string GuildId { get; init; }
        
        /// <summary>
        /// the channel it was sent from
        /// </summary>
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; init; }
        
        /// <summary>
        /// guild member data for the invoking user, including permissions
        /// </summary>
        [JsonPropertyName("member")]
        public Member Member { get; init; }
        
        /// <summary>
        /// user object for the invoking user, if invoked in a DM
        /// </summary>
        [JsonPropertyName("user")]
        public User User { get; init; }
        
        /// <summary>
        /// a continuation token for responding to the interaction
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; init; }
        
        /// <summary>
        /// read-only property, always 1
        /// </summary>
        [JsonPropertyName("version")]
        public int Version { get; init; }
        
        /// <summary>
        /// object	for components, the message they were attached to
        /// </summary>
        [JsonPropertyName("message")]
        public Message Message { get; init; }
    }
}