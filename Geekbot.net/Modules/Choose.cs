using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Choose : ModuleBase
    {
        private readonly Random rnd;
        public Choose(Random RandomClient)
        {
            rnd = RandomClient;
        }

        [Command("choose", RunMode = RunMode.Async), Summary("Let the bot make a choice for you.")]
        public async Task Command([Remainder, Summary("The choices, sepperated by a ;")] string choices)
        {
            var choicesArray = choices.Split(';');
            var choice = rnd.Next(choicesArray.Length);
            await ReplyAsync($"I choose **{choicesArray[choice]}**");
        }
    }
}