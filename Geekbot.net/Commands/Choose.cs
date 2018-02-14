using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
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
        [Remarks(CommandCategories.Helpers)]
        [Summary("Let the bot choose for you, seperate options with a semicolon.")]
        public async Task Command([Remainder] [Summary("option1;option2")]
            string choices)
        {
            try
            {
                var transDict = _translation.GetDict(Context);
                var choicesArray = choices.Split(';');
                var choice = new Random().Next(choicesArray.Length);
                await ReplyAsync(string.Format(transDict["Choice"], choicesArray[choice]));
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}