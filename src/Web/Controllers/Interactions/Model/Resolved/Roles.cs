namespace Geekbot.Web.Controllers.Interactions.Model.Resolved
{
    public record Roles
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Color { get; set; }
        public bool Hoist { get; set; }
        public int Position { get; set; }
        public string Permissions { get; set; }
        public bool Managed { get; set; }
        public bool Mentionable { get; set; }
        public RoleTag Tags { get; set; }
    }
}