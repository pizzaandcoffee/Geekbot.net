using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    public class Fortune : ModuleBase
    {
        private readonly IFortunes fortunes;
        public Fortune(IFortunes fortunes)
        {
            this.fortunes = fortunes;
        }
        
        [Command("fortune", RunMode = RunMode.Async), Summary("Get a random fortune")]
        public async Task GetAFortune()
        {
            await ReplyAsync(fortunes.GetRandomFortune());
        }
    }
}
