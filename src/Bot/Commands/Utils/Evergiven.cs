using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.GuildSettingsManager;
using HtmlAgilityPack;

namespace Geekbot.Bot.Commands.Utils
{
    public class Evergiven : GeekbotCommandBase
    {
        public Evergiven(IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
        }

        [Command("evergiven", RunMode = RunMode.Async)]
        [Summary("Check if the evergiven ship is still stuck in the suez canal")]
        public async Task GetStatus()
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine("Is that ship still stuck?");
                sb.AppendLine("**No!**");
                sb.AppendLine("It was stuck for 6 days, 3 hours and 38 minutes. It (probably) cost \"us\" $59 billion.");
                sb.AppendLine("You can follow it here: <https://www.vesselfinder.com/?imo=9811000>");
                
                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}