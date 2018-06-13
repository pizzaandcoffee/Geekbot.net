using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;

namespace Geekbot.net.Commands.Utils
{
    public class AvatarGetter : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public AvatarGetter(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("avatar", RunMode = RunMode.Async)]
        [Summary("Get someones avatar")]
        public async Task GetAvatar([Remainder] [Summary("user")] IUser user = null)
        {
            try
            {
                if (user == null) user = Context.User;
                var url = user.GetAvatarUrl().Replace("128", "1024");
                await ReplyAsync(url);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}