using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core.Media;

namespace Geekbot.Bot.Commands.Randomness
{
    public class Fortune : ModuleBase
    {
        private readonly IFortunesProvider _fortunes;

        public Fortune(IFortunesProvider fortunes)
        {
            _fortunes = fortunes;
        }

        [Command("fortune", RunMode = RunMode.Async)]
        [Summary("Get a random fortune")]
        public async Task GetAFortune()
        {
            await ReplyAsync(_fortunes.GetRandomFortune());
        }
    }
}