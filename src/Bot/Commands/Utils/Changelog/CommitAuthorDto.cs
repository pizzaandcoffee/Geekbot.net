using System;

namespace Geekbot.Bot.Commands.Utils.Changelog
{
    public class CommitAuthorDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}