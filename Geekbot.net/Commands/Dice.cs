using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Dice : ModuleBase
    {
        [Command("dice", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Randomness)]
        [Summary("Roll a dice.")]
        public async Task RollCommand([Remainder] [Summary("diceType")] string diceType = "1d20")
        {
            var splitedDices = diceType.Split("+");
            var dices = new List<DiceTypeDto>();
            var mod = 0;
            foreach (var i in splitedDices)
            {
                var dice = ToDice(i);
                if (dice.Sides != 0 && dice.Times != 0)
                {
                    dices.Add(dice);
                }
                else if (dice.Mod != 0)
                {
                    if (mod != 0)
                    {
                        await ReplyAsync("You can only have one mod");
                        return;
                    }

                    mod = dice.Mod;
                }
            }

            if (!dices.Any())
            {
                await ReplyAsync(
                    "That is not a valid dice, examples are: 1d20, 1d6, 2d6, 1d6+2, 1d6+2d8+1d20+6, etc...");
                return;
            }


            if (dices.Any(d => d.Times > 20))
            {
                await ReplyAsync("You can't throw more than 20 dices");
                return;
            }

            if (dices.Any(d => d.Sides > 120))
            {
                await ReplyAsync("A dice can't have more than 120 sides");
                return;
            }

            var rep = new StringBuilder();
            rep.AppendLine($":game_die: {Context.User.Mention}");
            rep.Append("**Result:** ");
            var resultStrings = new List<string>();
            var total = 0;
            var extraText = "";
            foreach (var dice in dices)
            {
                var results = new List<int>();
                for (var i = 0; i < dice.Times; i++)
                {
                    var roll = new Random().Next(1, dice.Sides);
                    total += roll;
                    results.Add(roll);
                    if (roll == dice.Sides) extraText = "**Critical Hit!**";
                    if (roll == 1) extraText = "**Critical Fail!**";
                }

                resultStrings.Add($"{dice.DiceType} ({string.Join(",", results)})");
            }

            rep.Append(string.Join(" + ", resultStrings));
            if (mod != 0)
            {
                rep.Append($" + {mod}");
                total += mod;
            }

            rep.AppendLine();
            rep.AppendLine($"**Total:** {total}");
            if (extraText != "") rep.AppendLine(extraText);
            await ReplyAsync(rep.ToString());
        }

        private DiceTypeDto ToDice(string dice)
        {
            var diceParts = dice.Split('d');
            if (diceParts.Length == 2
                && int.TryParse(diceParts[0], out var times)
                && int.TryParse(diceParts[1], out var max))
                return new DiceTypeDto
                {
                    DiceType = dice,
                    Times = times,
                    Sides = max
                };
            if (dice.Length == 1
                && int.TryParse(diceParts[0], out var mod))
                return new DiceTypeDto
                {
                    Mod = mod
                };
            return new DiceTypeDto();
        }
    }

    internal class DiceTypeDto
    {
        public string DiceType { get; set; }
        public int Times { get; set; }
        public int Sides { get; set; }
        public int Mod { get; set; }
    }
}