namespace Geekbot.net.Lib.Logger
{
    public class MessageDto
    {
        public MessageContent Message { get; set; }
        public IdAndName User { get; set; }
        public IdAndName Guild { get; set; }
        public IdAndName Channel { get; set; }
        
        public class MessageContent
        {
            public string Content { get; set; }
            public string Id { get; set; }
            public int Attachments { get; set; }
            public int ChannelMentions { get; set; }
            public int UserMentions { get; set; }
            public int RoleMentions { get; set; }
        }
        
        public class IdAndName
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}