using System;

namespace Geekbot.net.Commands.Utils.Changelog
{
    public class CommitAuthorDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}