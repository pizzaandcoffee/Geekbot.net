using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Roll : ModuleBase
    {
        [Command("roll"), Summary("Roll a number between 1 and 100.")]
        public async Task RollCommand()
        {
            var rnd = new Random();
            var number = rnd.Next(1, 100);
            await ReplyAsync(Context.Message.Author.Mention + ", you rolled " + number);
        }

        [Command("dice"), Summary("Roll a number between 1 and 100.")]
        public async Task DiceCommand()
        {
            var rnd = new Random();
            var number = rnd.Next(1, 6);
            await ReplyAsync(Context.Message.Author.Mention + ", you rolled " + number);
        }
    }
}