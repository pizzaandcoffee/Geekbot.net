using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class AvatarGetter : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public AvatarGetter(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("avatar", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Get someones avatar")]
        public async Task getAvatar([Remainder] [Summary("user")] IUser user = null)
        {
            try
            {
                if (user == null) user = Context.User;
                var url = user.GetAvatarUrl().Replace("128", "1024");
                await ReplyAsync(url);
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}