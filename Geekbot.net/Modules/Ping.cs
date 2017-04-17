using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Ping : ModuleBase
    {
        [Command("ping"), Summary("Pong.")]
        public async Task Say()
        {
            await Task.Delay(5000);
            await ReplyAsync("Pong");
        }
    }
}