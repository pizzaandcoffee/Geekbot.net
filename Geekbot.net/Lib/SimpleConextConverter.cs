using System;
using Discord.Commands;
using Discord.WebSocket;

namespace Geekbot.net.Lib
{
    public class SimpleConextConverter
    {
        public static MessageDto ConvertContext(ICommandContext context)
        {
            return ConvertSocketMessage((SocketMessage) context.Message);
        }
        public static MessageDto ConvertSocketMessage(SocketMessage message)
        {
            var channel = (SocketGuildChannel) message.Channel;
            return new MessageDto()
            {
                Message = new MessageDto.MessageContent()
                {
                    Content = message.Content,
                    Id = message.Id.ToString(),
                    Attachments = message.Attachments.Count,
                    ChannelMentions = message.MentionedChannels.Count,
                    UserMentions = message.MentionedUsers.Count,
                    RoleMentions = message.MentionedRoles.Count
                },
                User = new MessageDto.IdAndName()
                {
                    Id = message.Author.Id.ToString(),
                    Name = $"{message.Author.Username}#{message.Author.Discriminator}"
                },
                Guild = new MessageDto.IdAndName()
                {
                    Id = channel.Guild.Id.ToString(),
                    Name = channel.Guild.Name
                },
                Channel = new MessageDto.IdAndName()
                {
                    Id = channel.Id.ToString(),
                    Name = channel.Name
                },
            };
        }
        
    }
    
    
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