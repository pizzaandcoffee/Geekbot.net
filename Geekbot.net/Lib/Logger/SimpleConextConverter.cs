using Discord.Commands;
using Discord.WebSocket;

namespace Geekbot.net.Lib.Logger
{
    public class SimpleConextConverter
    {
        public static MessageDto ConvertContext(ICommandContext context)
        {
            return new MessageDto
            {
                Message = new MessageDto.MessageContent
                {
                    Content = context.Message.Content,
                    Id = context.Message.Id.ToString(),
                    Attachments = context.Message.Attachments.Count,
                    ChannelMentions = context.Message.MentionedChannelIds.Count,
                    UserMentions = context.Message.MentionedUserIds.Count,
                    RoleMentions = context.Message.MentionedRoleIds.Count
                },
                User = new MessageDto.IdAndName
                {
                    Id = context.User.Id.ToString(),
                    Name = $"{context.User.Username}#{context.User.Discriminator}"
                },
                Guild = new MessageDto.IdAndName
                {
                    Id = context.Guild.Id.ToString(),
                    Name = context.Guild.Name
                },
                Channel = new MessageDto.IdAndName
                {
                    Id = context.Channel.Id.ToString(),
                    Name = context.Channel.Name
                }
            };
        }
        public static MessageDto ConvertSocketMessage(SocketMessage message)
        {
            var channel = (SocketGuildChannel) message.Channel;
            return new MessageDto
            {
                Message = new MessageDto.MessageContent
                {
                    Content = message.Content,
                    Id = message.Id.ToString(),
                    Attachments = message.Attachments.Count,
                    ChannelMentions = message.MentionedChannels.Count,
                    UserMentions = message.MentionedUsers.Count,
                    RoleMentions = message.MentionedRoles.Count
                },
                User = new MessageDto.IdAndName
                {
                    Id = message.Author.Id.ToString(),
                    Name = $"{message.Author.Username}#{message.Author.Discriminator}"
                },
                Guild = new MessageDto.IdAndName
                {
                    Id = channel?.Guild?.Id.ToString(),
                    Name = channel?.Guild?.Name
                },
                Channel = new MessageDto.IdAndName
                {
                    Id = channel?.Id.ToString(),
                    Name = channel?.Name
                }
            };
        }
        
    }
}