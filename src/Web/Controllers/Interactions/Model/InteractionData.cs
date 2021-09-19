using System.Collections.Generic;

namespace Geekbot.Web.Controllers.Interactions.Model
{
    public record InteractionData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set;}
        public InteractionResolvedData Resolved { get; set; }
        public List<InteractionOption> Options { get; set; }
        public string TargetId { get; set; }
    }
}