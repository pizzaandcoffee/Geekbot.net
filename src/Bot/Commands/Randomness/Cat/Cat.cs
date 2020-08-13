using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Randomness.Cat
{
    public class Cat : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Cat(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("cat", RunMode = RunMode.Async)]
        [Summary("Return a random image of a cat.")]
        public async Task Say()
        {
            try
            {
                var response = await HttpAbstractions.Get<CatResponseDto>(new Uri("https://aws.random.cat/meow"));
                var eb = new EmbedBuilder
                {
                    ImageUrl = response.File
                };
                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}