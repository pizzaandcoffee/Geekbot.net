using System;
using Discord.Commands;
using Serilog;

namespace Geekbot.net.Lib
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogger logger;
//        private readonly IDMChannel botOwnerDmChannel;

        public ErrorHandler(ILogger logger /*, IDMChannel botOwnerDmChannel*/)
        {
            this.logger = logger;
//            this.botOwnerDmChannel = botOwnerDmChannel;
        }

        public async void HandleCommandException(Exception e, ICommandContext Context)
        {
            var errorMsg =
                $"Error Occured while executing \"{Context.Message.Content}\", executed by \"{Context.User.Username}\"";
            logger.Error(e, errorMsg);
//            await botOwnerDmChannel.SendMessageAsync($"{errorMsg}```{e.StackTrace}```");
//            await Context.Channel.SendMessageAsync("Something went wrong...");
        }
    }

    public interface IErrorHandler
    {
        void HandleCommandException(Exception e, ICommandContext Context);
    }
}