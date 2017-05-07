using System.Threading.Tasks;
using System;
using Discord;
using Discord.Commands;
using Geekbot.net.Lambda;

namespace Geekbot.net.Modules
{
    public class Lambda : ModuleBase
    {
        [Command("λ", RunMode = RunMode.Async), Summary("λ")]
        public async Task SICP()
        {
            await ReplyAsync(autism.sicp("https://a.pomf.cat/tgdevn.png"));
        }

        [Command("kek", RunMode = RunMode.Async), Summary("λ")]
        public async Task kek()
        {
            await ReplyAsync(kekistan.kek("top"));
        }

    }
}
