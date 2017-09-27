using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.Media;

namespace Geekbot.net.Modules
{
    public class Fortune : ModuleBase
    {
        private readonly IFortunesProvider fortunes;

        public Fortune(IFortunesProvider fortunes)
        {
            this.fortunes = fortunes;
        }

        [Command("fortune", RunMode = RunMode.Async)]
        [Summary("Get a random fortune")]
        public async Task GetAFortune()
        {
            await ReplyAsync(fortunes.GetRandomFortune());
        }
    }
}