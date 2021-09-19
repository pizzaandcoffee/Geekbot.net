using System.Collections.Generic;

namespace Geekbot.Web.Controllers.Interactions.Model
{
    public record InteractionOption
    {
        public string Name { get; set; }
        public ApplicationCommandOption Type { get; set; }
        public string Value { get; set; }
        public List<InteractionOption> Options { get; set; }
    }
}