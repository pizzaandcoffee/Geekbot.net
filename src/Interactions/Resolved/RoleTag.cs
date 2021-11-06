using System.Text.Json.Serialization;

namespace Geekbot.Interactions.Resolved
{
    public record RoleTag
    {
        [JsonPropertyName("bot_id")]
        public string BotId { get; set; }
        
        [JsonPropertyName("integration_id")]
        public string IntegrationId { get; set; }
        
        [JsonPropertyName("premium_subscriber")]
        public bool PremiumSubscriber { get; set; }
    }
}