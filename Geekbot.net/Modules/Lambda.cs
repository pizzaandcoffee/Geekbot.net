using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lambda;

namespace Geekbot.net.Modules
{
    public class Lambda : ModuleBase
    {
        [Command("λ", RunMode = RunMode.Async), Summary("λ")]
        public async Task SICP()
        {
            await ReplyAsync(autism.sicp("https://files.catbox.moe/39m1o7.png"));
        }

        [Command("kek", RunMode = RunMode.Async), Summary("kek")]
        public async Task kek()
        {
            await ReplyAsync(kekistan.kek("top"));
        }

    }
}
