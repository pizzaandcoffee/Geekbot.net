using System.Collections.Generic;
using Geekbot.Core.Interactions.ApplicationCommand;

namespace Geekbot.Web.Controllers.Interactions.Model
{
    public record InteractionRegistrationOperations
    {
        public List<Command> Create { get; set; } = new();
        public Dictionary<string, Command> Update { get; set; } = new();
        public List<string> Remove { get; set; } = new();
    }
}