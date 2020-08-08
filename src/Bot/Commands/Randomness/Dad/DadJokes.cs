using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Randomness.Dad
{
    public class DadJokes : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public DadJokes(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        [Command("dad", RunMode = RunMode.Async)]
        [Summary("A random dad joke")]
        public async Task Say()
        {
            try
            {
                var response = await HttpAbstractions.Get<DadJokeResponseDto>(new Uri("https://icanhazdadjoke.com/"));
                await ReplyAsync(response.Joke);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}