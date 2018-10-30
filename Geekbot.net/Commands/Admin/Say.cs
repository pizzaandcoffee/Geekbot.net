using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;

namespace Geekbot.net.Commands.Admin
{
    public class Say : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Say(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("say", RunMode = RunMode.Async)]
        [Summary("Say Something.")]
        public async Task Echo([Remainder] [Summary("What?")] string echo)
        {
            try
            {
                await Context.User.SendMessageAsync("the `!say` command has been disabled since 2018/10/30");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}