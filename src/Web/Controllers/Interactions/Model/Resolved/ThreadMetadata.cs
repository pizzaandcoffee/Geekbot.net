using System;

namespace Geekbot.Web.Controllers.Interactions.Model.Resolved
{
    public record ThreadMetadata
    {
        public bool Archived { get; set; }
        public int AutoArchiveDuration { get; set; }
        public DateTime ArchiveTimestamp { get; set; }
        public bool Locked { get; set; }
        public bool Invitable { get; set; }
    }
}