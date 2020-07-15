using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;

namespace Geekbot.net.Commands.Randomness.Dog
{
    public class Dog : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Dog(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("dog", RunMode = RunMode.Async)]
        [Summary("Return a random image of a dog.")]
        public async Task Say()
        {
            try
            {
                var response = await HttpAbstractions.Get<DogResponseDto>(new Uri("http://random.dog/woof.json"));
                var eb = new EmbedBuilder
                {
                    ImageUrl = response.Url
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