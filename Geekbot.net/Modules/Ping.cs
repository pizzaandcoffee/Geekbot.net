using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Ping : ModuleBase
    {
        [Command("ping"), Summary("Pong.")]
        public async Task Say()
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync("Pong");
        }
    }
}