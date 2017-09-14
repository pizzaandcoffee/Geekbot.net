using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using StackExchange.Redis;

namespace Geekbot.net.Modules
{
    public class Dice : ModuleBase
    {
        private readonly Random rnd;
        public Dice(Random RandomClient)
        {
            rnd = RandomClient;
        }

        [Command("dice", RunMode = RunMode.Async), Summary("Roll a dice.")]
        public async Task RollCommand([Remainder, Summary("1d20, 1d6, 2d3, etc...")] string diceType = "1d6")
        {
            var dice = diceType.Split("d");

            if (dice.Length != 2
                || !int.TryParse(dice[0], out int times)
                || !int.TryParse(dice[1], out int max))
            {
                await ReplyAsync("That is not a valid dice, examples are: 1d20, 1d6, 2d10, 5d12, etc...");
                return;
            }
            Console.WriteLine($"Max: {max} - Times {times}");
            if (times > 10 && !(times < 0))
            {
                await ReplyAsync("You can only roll between 1 and 10 dices");
                return;
            }
            if (max > 100 && !(max < 1))
            {
                await ReplyAsync("The dice must have between 1 and 100 sides");
                return;
            }
            var eb = new EmbedBuilder();
            eb.WithAuthor(new EmbedAuthorBuilder()
                .WithIconUrl(Context.User.GetAvatarUrl())
                .WithName(Context.User.Username));
            eb.WithColor(new Color(133, 189, 219));
            eb.Title = $":game_die:  Dice Roll - Type {diceType} :game_die:";
            for (var i = 0; i < times; i++)
            {
                eb.AddInlineField($"Dice {i+1}", rnd.Next(1, max));
            }
            await ReplyAsync("", false, eb.Build());
        }
    }
}