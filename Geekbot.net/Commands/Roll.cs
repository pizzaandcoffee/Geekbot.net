using System;
using System.Threading.Tasks;
using Discord.Commands;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class Roll : ModuleBase
    {
        private readonly IDatabase redis;
        private readonly Random rnd;

        public Roll(IDatabase redis, Random RandomClient)
        {
            this.redis = redis;
            rnd = RandomClient;
        }

        [Command("roll", RunMode = RunMode.Async)]
        [Summary("Roll a number between 1 and 100.")]
        public async Task RollCommand([Remainder] [Summary("stuff...")] string stuff = "nothing")
        {
            var number = rnd.Next(1, 100);
            var guess = 1000;
            int.TryParse(stuff, out guess);
            if (guess <= 100 && guess > 0)
            {
                await ReplyAsync($"{Context.Message.Author.Mention} you rolled {number}, your guess was {guess}");
                if (guess == number)
                {
                    await ReplyAsync($"Congratulations {Context.User.Username}, your guess was correct!");
                    redis.HashIncrement($"{Context.Guild.Id}:Rolls", Context.User.Id.ToString()); 
                }
            }
            else
            {
                await ReplyAsync(Context.Message.Author.Mention + ", you rolled " + number);
            }
        }
    }
}