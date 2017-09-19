using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Dice : ModuleBase
    {
        private readonly Random rnd;

        public Dice(Random RandomClient)
        {
            rnd = RandomClient;
        }

        [Command("dice", RunMode = RunMode.Async)]
        [Summary("Roll a dice.")]
        public async Task RollCommand([Remainder] [Summary("diceType")] string diceType = "1d6")
        {
            var dice = diceType.Split("d");
            var maxAndMod = dice[1].Split("+");

            if (dice.Length != 2
                || !int.TryParse(dice[0], out int times)
                || !int.TryParse(maxAndMod[0], out int max))
            {
                await ReplyAsync("That is not a valid dice, examples are: 1d20, 1d6, 2d10, 5d12, 1d20+5, 1d6+2, etc...");
                return;
            }
            
            int modifier = 0;
            if (maxAndMod.Length == 2 && !int.TryParse(maxAndMod[1], out modifier) || maxAndMod.Length > 2)
            {
                await ReplyAsync("That is not a valid dice, examples are: 1d20, 1d6, 2d10, 5d12, 1d20+5, 1d6+2, etc...");
                return;
            }
            
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
            
            var total = 0;
            for (var i = 0; i < times; i++)
            {
                var roll = rnd.Next(1, max);
                eb.AddInlineField($"Dice {i + 1}", roll);
                total = total + roll;
            }
            
            eb.AddField("Total", modifier == 0 ? $"{total}" : $"{total + modifier} ({total} +{modifier})");
            await ReplyAsync("", false, eb.Build());
        }
    }
}
