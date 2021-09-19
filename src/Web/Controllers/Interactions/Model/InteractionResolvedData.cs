using System.Collections.Generic;
using Discord;
using Geekbot.Web.Controllers.Interactions.Model.Resolved;

namespace Geekbot.Web.Controllers.Interactions.Model
{
    public class InteractionResolvedData
    {
        public Dictionary<string, User> Users { get; set; }
        public Dictionary<string, Member> Members { get; set; }
        public Dictionary<string, Roles> Roles { get; set; }
        public Dictionary<string, Channel> Channels { get; set; }
        // public Dictionary<string, IMessage> Messages { get; set; }
    }
}