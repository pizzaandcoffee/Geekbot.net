using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.Media;

namespace Geekbot.Bot.Commands.Randomness
{
    public class RandomAnimals : TransactionModuleBase
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
            await ReplyAsync("", false, Eb(_mediaProvider.GetMedia(MediaType.Panda)));
        }

        [Command("croissant", RunMode = RunMode.Async)]
        [Alias("gipfeli")]
        [Summary("Get a random croissant image")]
        public async Task Croissant()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetMedia(MediaType.Croissant)));
        }

        [Command("pumpkin", RunMode = RunMode.Async)]
        [Summary("Get a random pumpkin image")]
        public async Task Pumpkin()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetMedia(MediaType.Pumpkin)));
        }

        [Command("squirrel", RunMode = RunMode.Async)]
        [Summary("Get a random squirrel image")]
        public async Task Squirrel()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetMedia(MediaType.Squirrel)));
        }

        [Command("turtle", RunMode = RunMode.Async)]
        [Summary("Get a random turtle image")]
        public async Task Turtle()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetMedia(MediaType.Turtle)));
        }
        
        [Command("penguin", RunMode = RunMode.Async)]
        [Alias("pengu")]
        [Summary("Get a random penguin image")]
        public async Task Penguin()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetMedia(MediaType.Penguin)));
        }
        
        [Command("fox", RunMode = RunMode.Async)]
        [Summary("Get a random fox image")]
        public async Task Fox()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetMedia(MediaType.Fox)));
        }
        
        [Command("dab", RunMode = RunMode.Async)]
        [Summary("Get a random dab image")]
        public async Task Dab()
        {
            await ReplyAsync("", false, Eb(_mediaProvider.GetMedia(MediaType.Dab)));
        }

        private static Embed Eb(string image)
        {
            return new EmbedBuilder {ImageUrl = image}.Build();
        }
    }
}