using System.Collections.Generic;

namespace Geekbot.net.Commands.Utils.Poll
{
    internal class PollDataDto
    {
        public ulong Creator { get; set; }
        public ulong MessageId { get; set; }
        public bool IsFinshed { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; }
    }
}