using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;

namespace Geekbot.net.Commands.Randomness.Chuck
{
    public class ChuckNorrisJokes : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public ChuckNorrisJokes(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        [Command("chuck", RunMode = RunMode.Async)]
        [Summary("A random chuck norris joke")]
        public async Task Say()
        {
            try
            {
                try
                {
                    var response = await HttpAbstractions.Get<ChuckNorrisJokeResponseDto>(new Uri("https://api.chucknorris.io/jokes/random"));
                    await ReplyAsync(response.Value);
                }
                catch (HttpRequestException)
                {
                    await ReplyAsync("Api down...");
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}