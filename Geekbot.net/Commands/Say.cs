using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
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
        [Remarks(CommandCategories.Admin)]
        [Summary("Say Something.")]
        public async Task Echo([Remainder] [Summary("What?")] string echo)
        {
            try
            {
                await Context.Message.DeleteAsync();
                await ReplyAsync(echo);
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}