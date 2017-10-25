using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Help : ModuleBase
    {
        private readonly CommandService _commands;
        private readonly IErrorHandler _errorHandler;

        public Help(CommandService commands, IErrorHandler errorHandler)
        {
            _commands = commands;
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
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}