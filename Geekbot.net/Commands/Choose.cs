using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Choose : ModuleBase
    {
        private readonly Random rnd;

        public Choose(Random RandomClient)
        {
            rnd = RandomClient;
        }

        [Command("choose", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Let the bot choose for you, seperate options with a semicolon.")]
        public async Task Command([Remainder] [Summary("option1;option2")] string choices)
        {
            var choicesArray = choices.Split(';');
            var choice = rnd.Next(choicesArray.Length);
            await ReplyAsync($"I choose **{choicesArray[choice]}**");
        }
    }
}