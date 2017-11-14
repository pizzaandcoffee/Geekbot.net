using System;
using Discord.Commands;
using Serilog;
using Utf8Json;

namespace Geekbot.net.Lib
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogger _logger;

        public ErrorHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void HandleCommandException(Exception e, ICommandContext Context, string errorMessage = "Something went wrong :confused:")
        {
            try
            {
                var errorObj = new ErrorObject()
                {
                    Message = new ErrorMessage()
                    {
                        Content = Context.Message.Content,
                        Id = Context.Message.Id.ToString(),
                        Attachments = Context.Message.Attachments.Count,
                        ChannelMentions = Context.Message.MentionedChannelIds.Count,
                        UserMentions = Context.Message.MentionedUserIds.Count,
                        RoleMentions = Context.Message.MentionedRoleIds.Count
                    },
                    User = new IdAndName()
                    {
                        Id = Context.User.Id.ToString(),
                        Name = $"{Context.User.Username}#{Context.User.Discriminator}"
                    },
                    Guild = new IdAndName()
                    {
                        Id = Context.Guild.Id.ToString(),
                        Name = Context.Guild.Name
                    },
                    Channel = new IdAndName()
                    {
                        Id = Context.Channel.Id.ToString(),
                        Name = Context.Channel.Name
                    },
                    TimeStamp = DateTime.Now.ToString()
                };
                var errorJson = JsonSerializer.ToJsonString(errorObj);
                _logger.Error(e, errorJson);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Context.Channel.SendMessageAsync(errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Errorception");
            }
        }
        
        public class ErrorObject
        {
            public ErrorMessage Message { get; set; }
            public IdAndName User { get; set; }
            public IdAndName Guild { get; set; }
            public IdAndName Channel { get; set; }
            public string TimeStamp { get; set; }
        }
        
        public class ErrorMessage
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

    public interface IErrorHandler
    {
        void HandleCommandException(Exception e, ICommandContext Context, string errorMessage = "Something went wrong :confused:");
    }
}