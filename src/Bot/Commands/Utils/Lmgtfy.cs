using System;
using System.Threading.Tasks;
using System.Web;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Utils
{
    public class Lmgtfy : TransactionModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Lmgtfy(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        [Command("lmgtfy", RunMode = RunMode.Async)]
        [Summary("Get a 'Let me google that for you' link")]
        public async Task GetUrl([Remainder] [Summary("question")] string question)
        {
            try
            {
                var encoded = HttpUtility.UrlEncode(question).Replace("%20", "+");
                await Context.Channel.SendMessageAsync($"<https://lmgtfy.com/?q={encoded}>");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}