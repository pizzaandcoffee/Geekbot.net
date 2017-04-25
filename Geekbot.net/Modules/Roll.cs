using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.IClients;

namespace Geekbot.net.Modules
{
    public class Roll : ModuleBase
    {
        private readonly IRedisClient redis;
        private readonly IRandomClient rnd;
        public Roll(IRedisClient redisClient, IRandomClient randomClient)
        {
            redis = redisClient;
            rnd = randomClient;
        }

        [Command("roll"), Summary("Roll a number between 1 and 100.")]
        public async Task RollCommand([Remainder, Summary("stuff...")] string stuff = "nothing")
        {
            var number = rnd.Client.Next(1, 100);
            var guess = 1000;
            int.TryParse(stuff, out guess);
            if (guess <= 100 && guess > 0)
            {
                await ReplyAsync($"{Context.Message.Author.Mention} you rolled {number}, your guess was {guess}");
                if (guess == number)
                {
                    await ReplyAsync($"Congratulations {Context.User.Username}, your guess was correct!");
                    var key = $"{Context.Guild.Id}-{Context.User.Id}-correctRolls";
                    var messages = (int)redis.Client.StringGet(key);
                    redis.Client.StringSet(key, (messages + 1).ToString());
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
            var number = rnd.Client.Next(1, max);
            await ReplyAsync(Context.Message.Author.Mention + ", you rolled " + number);
        }
    }
}