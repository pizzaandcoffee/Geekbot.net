using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.Response
{
    /// <summary>
    /// Interactions--both receiving and responding--are webhooks under the hood. So responding to an Interaction is just like sending a webhook request!
    /// </summary>
    /// <see href="https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-interaction-response-structure"/>
    public record InteractionResponse
    {
        /// <summary>
        /// the type of response
        /// </summary>
        [JsonPropertyName("type")]
        public InteractionResponseType Type { get; set; }
        
        /// <summary>
        ///	an optional response message
        /// </summary>
        [JsonPropertyName("data")]
        public InteractionResponseData Data { get; set; }
    }
}