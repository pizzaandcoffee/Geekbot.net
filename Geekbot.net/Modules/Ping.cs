using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Ping : ModuleBase
    {
        [Command("ping"), Summary("Pong.")]
        public async Task Say()
        {
            await ReplyAsync("Pong");
        }

        [Command("hui"), Summary("hui!!!.")]
        public async Task Hui()
        {
            await ReplyAsync("hui!!!");
        }
    }
}