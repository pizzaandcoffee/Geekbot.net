using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Ping : ModuleBase
    {
        [Command("👀"), Summary("Look at the bot.")]
        public async Task Eyes()
        {
            await ReplyAsync("S... Stop looking at me... baka!");
        }
    }
}