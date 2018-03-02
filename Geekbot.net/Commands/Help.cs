using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Help : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Help(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("help", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("List all Commands")]
        public async Task GetHelp()
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("For a list of all commands, please visit the following page");
                sb.AppendLine("https://geekbot.pizzaandcoffee.rocks/commands");
                var dm = await Context.User.GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync(sb.ToString());
                Context.Message.AddReactionAsync(new Emoji("✅"));
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}