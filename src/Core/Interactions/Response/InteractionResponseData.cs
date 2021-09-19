using System.Collections.Generic;
using System.Text.Json.Serialization;
using Discord;
using Geekbot.Core.Interactions.MessageComponents;
using Embed = Geekbot.Core.Interactions.Embed.Embed;

namespace Geekbot.Core.Interactions.Response
{
    /// <remarks>
    /// Not all message fields are currently supported.
    /// </remarks>
    /// <see href="https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-interaction-callback-data-structure"/>
    public record InteractionResponseData
    {
        /// <summary>
        /// is the response TTS
        /// </summary>
        [JsonPropertyName("tts")]
        public bool Tts { get; set; } = false;
        
        /// <summary>
        /// message content
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        /// <summary>
        /// supports up to 10 embeds
        /// </summary>
        [JsonPropertyName("embeds")]
        public List<Embed.Embed> Embeds { get; set; }
        
        /// <summary>
        /// allowed mentions object
        /// </summary>
        [JsonPropertyName("allowed_mentions")]
        public AllowedMentions AllowedMentions { get; set; }
        
        /// <summary>
        /// interaction callback data flags
        /// </summary>
        [JsonPropertyName("flags")]
        public int Flags { get; set; }
        
        /// <summary>
        /// message components
        /// </summary>
        [JsonPropertyName("components")]
        public List<Component> Components { get; set; }
    }
}