namespace Geekbot.Web.Controllers.Interactions.Model.Resolved
{
    public record Channel
    {
        public string Id { get; set; }
        public ChannelType Type { get; set; }
        public string Name { get; set; }
        public string ParentId { get; set; }
        public ThreadMetadata ThreadMetadata { get; set; }
        public string Permissions { get; set; }
    }
}