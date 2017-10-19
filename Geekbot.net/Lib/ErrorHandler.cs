using System;
using Discord.Commands;
using Serilog;

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
            var errorMsg =
                $"Error Occured while executing \"{Context.Message.Content}\", executed by \"{Context.User.Username}\"";
            _logger.Error(e, errorMsg);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Context.Channel.SendMessageAsync(errorMessage);
            }

        }
    }

    public interface IErrorHandler
    {
        void HandleCommandException(Exception e, ICommandContext Context, string errorMessage = "Something went wrong :confused:");
    }
}