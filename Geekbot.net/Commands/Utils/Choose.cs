using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Localization;

namespace Geekbot.net.Commands.Utils
{
    public class Choose : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly ITranslationHandler _translation;

        public Choose(IErrorHandler errorHandler, ITranslationHandler translation)
        {
            _errorHandler = errorHandler;
            _translation = translation;
        }

        [Command("choose", RunMode = RunMode.Async)]
        [Summary("Let the bot choose for you, seperate options with a semicolon.")]
        public async Task Command([Remainder] [Summary("option1;option2")]
            string choices)
        {
            try
            {
                var transContext = await _translation.GetGuildContext(Context);
                var choicesArray = choices.Split(';');
                var choice = new Random().Next(choicesArray.Length);
                await ReplyAsync(transContext.GetString("Choice", choicesArray[choice]));
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}