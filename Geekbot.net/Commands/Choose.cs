using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Choose : ModuleBase
    {
        private readonly Random _rnd;
        private readonly IErrorHandler _errorHandler;

        public Choose(Random RandomClient, IErrorHandler errorHandler)
        {
            _rnd = RandomClient;
            _errorHandler = errorHandler;
        }

        [Command("choose", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Let the bot choose for you, seperate options with a semicolon.")]
        public async Task Command([Remainder] [Summary("option1;option2")] string choices)
        {
            try
            {
                var choicesArray = choices.Split(';');
                var choice = _rnd.Next(choicesArray.Length);
                await ReplyAsync($"I choose **{choicesArray[choice]}**");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}