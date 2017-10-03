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
        [Summary("List all Commands")]
        public async Task GetHelp()
        {
            try
            {
                var sb = new StringBuilder();
//                sb.AppendLine("```");
//                sb.AppendLine("**Geekbot Command list**");
//                sb.AppendLine("");
//                sb.AppendLine(tp("Name", 15) + tp("Parameters", 19) + "Description");
//                foreach (var cmd in _commands.Commands)
//                {
//                    var param = string.Join(", !", cmd.Aliases);
//                    if (!param.Contains("admin"))
//                        if (cmd.Parameters.Any())
//                            sb.AppendLine(tp(param, 15) +
//                                          tp(string.Join(" ", cmd.Parameters.Select(e => e.Summary)), 19) +
//                                          cmd.Summary);
//                        else
//                            sb.AppendLine(tp(param, 34) + cmd.Summary);
//                }
//                sb.AppendLine("```");
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

        // Table Padding, short function name because of many usages
        private string tp(string text, int shouldHave)
        {
            return text.PadRight(shouldHave);
        }
    }
}