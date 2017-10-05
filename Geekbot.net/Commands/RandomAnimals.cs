using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.Media;

namespace Geekbot.net.Commands
{
    public class RandomAnimals : ModuleBase
    {
        private readonly IMediaProvider _mediaProvider;
        
        public RandomAnimals(IMediaProvider mediaProvider)
        {
            _mediaProvider = mediaProvider;
        }
        
        [Command("panda", RunMode = RunMode.Async)]
        [Summary("Get a random panda image")]
        public async Task panda()
        {
            await ReplyAsync(_mediaProvider.getPanda());
        }
        
        [Command("croissant", RunMode = RunMode.Async)]
        [Alias("gipfeli")]
        [Summary("Get a random croissant image")]
        public async Task croissant()
        {
            await ReplyAsync(_mediaProvider.getCrossant());
        }
        
        [Command("pumpkin", RunMode = RunMode.Async)]
        [Summary("Get a random pumpkin image")]
        public async Task pumpkin()
        {
            await ReplyAsync(_mediaProvider.getPumpkin());
        }
        
        [Command("squirrel", RunMode = RunMode.Async)]
        [Summary("Get a random squirrel image")]
        public async Task squirrel()
        {
            await ReplyAsync(_mediaProvider.getSquirrel());
        }
        
        [Command("turtle", RunMode = RunMode.Async)]
        [Summary("Get a random turtle image")]
        public async Task turtle()
        {
            await ReplyAsync(_mediaProvider.getTurtle());
        }
    }
}