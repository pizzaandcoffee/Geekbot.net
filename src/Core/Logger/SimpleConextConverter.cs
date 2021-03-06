﻿using Discord.Commands;
using Discord.WebSocket;

namespace Geekbot.Core.Logger
{
    public class SimpleConextConverter
    {
        public static MessageDto ConvertContext(ICommandContext context)
        {
            return new MessageDto
            {
                Message = new MessageDto.MessageContent
                {
                    Content = context.Message.Content, // Only when an error occurs, including for diagnostic reason 
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
                    Id = context.Guild?.Id.ToString(),
                    Name = context.Guild?.Name
                },
                Channel = new MessageDto.IdAndName
                {
                    Id = context.Channel?.Id.ToString() ?? context.User.Id.ToString(),
                    Name = context.Channel?.Name ?? "DM-Channel"
                }
            };
        }
        public static MessageDto ConvertSocketMessage(SocketMessage message, bool isPrivate = false)
        {
            var channel = isPrivate ? null : (SocketGuildChannel) message.Channel;
            return new MessageDto
            {
                Message = new MessageDto.MessageContent
                {
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
                    Id = channel?.Id.ToString() ?? message.Author.Id.ToString(),
                    Name = channel?.Name  ?? "DM-Channel"
                }
            };
        }
        
    }
}