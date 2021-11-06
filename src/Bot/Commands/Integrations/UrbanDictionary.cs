using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Integrations
{
    public class UrbanDictionary : TransactionModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public UrbanDictionary(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("urban", RunMode = RunMode.Async)]
        [Summary("Lookup something on urban dictionary")]
        public async Task UrbanDefine([Remainder] [Summary("word")] string word)
        {
            try
            {
                var eb = await Geekbot.Commands.UrbanDictionary.UrbanDictionary.Run(word);
                if (eb == null)
                {
                    await ReplyAsync("That word hasn't been defined...");
                    return;
                }

                await ReplyAsync(string.Empty, false, eb.ToDiscordNetEmbed().Build());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}