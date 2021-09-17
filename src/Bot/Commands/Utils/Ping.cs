using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core;

namespace Geekbot.Bot.Commands.Utils
{
    public class Ping : TransactionModuleBase
    {
        [Command("👀", RunMode = RunMode.Async)]
        [Summary("Look at the bot.")]
        public async Task Eyes()
        {
            await ReplyAsync("S... Stop looking at me... baka!");
        }
    }
}