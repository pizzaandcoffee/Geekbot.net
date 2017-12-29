using System;
using Discord.Commands;
using Discord.WebSocket;

namespace Geekbot.net.Lib
{
    public class SimpleConextConverter
    {
        public static MessageDto ConvertContext(ICommandContext context)
        {
            return new MessageDto()
            {
                Message = new MessageDto.MessageContent()
                {
                    Content = context.Message.Content,
                    Id = context.Message.Id.ToString(),
                    Attachments = context.Message.Attachments.Count,
                    ChannelMentions = context.Message.MentionedChannelIds.Count,
                    UserMentions = context.Message.MentionedUserIds.Count,
                    RoleMentions = context.Message.MentionedRoleIds.Count
                },
                User = new MessageDto.IdAndName()
                {
                    Id = context.User.Id.ToString(),
                    Name = $"{context.User.Username}#{context.User.Discriminator}"
                },
                Guild = new MessageDto.IdAndName()
                {
                    Id = context.Guild.Id.ToString(),
                    Name = context.Guild.Name
                },
                Channel = new MessageDto.IdAndName()
                {
                    Id = context.Channel.Id.ToString(),
                    Name = context.Channel.Name
                },
                TimeStamp = DateTime.Now.ToString()
            };
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
                TimeStamp = DateTime.Now.ToString()
            };
        }
        
    }
    
    
    public class MessageDto
    {
        public MessageContent Message { get; set; }
        public IdAndName User { get; set; }
        public IdAndName Guild { get; set; }
        public IdAndName Channel { get; set; }
        public string TimeStamp { get; set; }
        
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