using System;
using System.Collections.Generic;

namespace Geekbot.Web.Controllers.Interactions.Model.Resolved
{
    public record Member
    {
        // public User User { get; set; }
        public string Nick { get; set; }
        public List<string> Roles { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime PremiumSince { get; set; }
        public bool Pending { get; set; }
        public string Permissions { get; set; }
    }
}