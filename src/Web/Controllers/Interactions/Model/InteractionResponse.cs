using System.Text.Json.Serialization;

namespace Geekbot.Web.Controllers.Interactions.Model
{
    public record InteractionResponse
    {
        [JsonPropertyName("type")]
        public InteractionResponseType Type { get; set; }
        
        [JsonPropertyName("data")]
        public InteractionData Data { get; set; }
    }
}