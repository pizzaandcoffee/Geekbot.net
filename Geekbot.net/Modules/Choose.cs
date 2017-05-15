using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.IClients;

namespace Geekbot.net.Modules
{
    public class Choose : ModuleBase
    {
        private readonly IRandomClient rnd;
        public Choose(IRandomClient randomClient)
        {
            rnd = randomClient;
        }

        [Command("choose", RunMode = RunMode.Async), Summary("Let the bot make a choice for you.")]
        public async Task Command([Remainder, Summary("The choices, sepperated by a ;")] string choices)
        {
            var choicesArray = choices.Split(';');
            var choice = rnd.Client.Next(choicesArray.Length);
            await ReplyAsync($"I choose **{choicesArray[choice]}**");
        }
    }
}