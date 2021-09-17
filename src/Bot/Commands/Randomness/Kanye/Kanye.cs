using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Randomness.Kanye
{
    public class Kanye : TransactionModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Kanye(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        [Command("kanye", RunMode = RunMode.Async)]
        [Summary("A random kayne west quote")]
        public async Task Say()
        {
            try
            {
                var response = await HttpAbstractions.Get<KanyeResponseDto>(new Uri("https://api.kanye.rest/"));
                await ReplyAsync(response.Quote);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}