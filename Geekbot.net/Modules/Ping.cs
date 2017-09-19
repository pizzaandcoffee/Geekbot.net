using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Ping : ModuleBase
    {
        [Command("👀", RunMode = RunMode.Async)]
        [Summary("Look at the bot.")]
        public async Task Eyes()
        {
            await ReplyAsync("S... Stop looking at me... baka!");
        }
    }
}