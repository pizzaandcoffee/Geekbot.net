using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Media;

namespace Geekbot.net.Commands.Randomness
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
        public async Task Panda()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetPanda()));
        }

        [Command("croissant", RunMode = RunMode.Async)]
        [Alias("gipfeli")]
        [Summary("Get a random croissant image")]
        public async Task Croissant()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetCrossant()));
        }

        [Command("pumpkin", RunMode = RunMode.Async)]
        [Summary("Get a random pumpkin image")]
        public async Task Pumpkin()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetPumpkin()));
        }

        [Command("squirrel", RunMode = RunMode.Async)]
        [Summary("Get a random squirrel image")]
        public async Task Squirrel()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetSquirrel()));
        }

        [Command("turtle", RunMode = RunMode.Async)]
        [Summary("Get a random turtle image")]
        public async Task Turtle()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetTurtle()));
        }
        
        [Command("pinguin", RunMode = RunMode.Async)]
        [Alias("pingu")]
        [Summary("Get a random pinguin image")]
        public async Task Pinguin()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetPinguin()));
        }
        
        [Command("fox", RunMode = RunMode.Async)]
        [Summary("Get a random fox image")]
        public async Task Fox()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetFox()));
        }

        private EmbedBuilder Eb(string image)
        {
            var eb = new EmbedBuilder();
            eb.ImageUrl = image;
            return eb;
        }
    }
}