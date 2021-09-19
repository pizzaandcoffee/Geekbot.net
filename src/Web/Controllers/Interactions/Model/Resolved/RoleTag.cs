namespace Geekbot.Web.Controllers.Interactions.Model.Resolved
{
    public record RoleTag
    {
        public string BotId { get; set; }
        public string IntegrationId { get; set; }
        public bool PremiumSubscriber { get; set; }
    }
}