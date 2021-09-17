using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Utils
{
    public class AvatarGetter : TransactionModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public AvatarGetter(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("avatar", RunMode = RunMode.Async)]
        [Summary("Get someones avatar")]
        public async Task GetAvatar([Remainder, Summary("@someone")] IUser user = null)
        {
            try
            {
                user ??= Context.User;
                var url = user.GetAvatarUrl(ImageFormat.Auto, 1024);
                await ReplyAsync(url);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}