using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Roll : ModuleBase
    {
        [Command("roll"), Summary("Roll a number between 1 and 100.")]
        public async Task RollCommand([Remainder, Summary("stuff...")] string stuff = "nothing")
        {
            var rnd = new Random();
            var number = rnd.Next(1, 100);
            var guess = 1000;
            int.TryParse(stuff, out guess);
            if (guess <= 100 && guess > 0)
            {
                await ReplyAsync($"{Context.Message.Author.Mention} you rolled {number}, your guess was {guess}");
                if (guess == number)
                {
                    await ReplyAsync($"Congratulations {Context.User.Username}, your guess was correct!");
                }
            }
            else
            {
                await ReplyAsync(Context.Message.Author.Mention + ", you rolled " + number);
            }
        }

        [Command("dice"), Summary("Roll a dice")]
        public async Task DiceCommand([Summary("The highest number on the dice")] int max = 6)
        {
            var rnd = new Random();
            var number = rnd.Next(1, max);
            await ReplyAsync(Context.Message.Author.Mention + ", you rolled " + number);
        }
    }
}