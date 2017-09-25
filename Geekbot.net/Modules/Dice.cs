using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task RollCommand([Remainder] [Summary("diceType")] string inputString = "1d20")
        {
            // convert input into real objects, wihtout computation
            var tokens = inputString.Split("+");

            var computableTokens = tokens.Select(token => toComputation(token)).Where(t => t != null).ToList();

            if (!computableTokens.Any())
            {
                await ReplyAsync(
                    "That is not a valid dice, examples are: 1d20, 1d6, 2d6, 1d6+2, 1d6+2d8+1d20+6, etc...");
                return;
            }

            // get computed results
            var computedResults = computableTokens.Select(die => new ComputedResult { Die = die, Roll = Compute(die) }).ToList();
            
            // validate mod location and duplication
            // ?

            // format computed results
            var result = ConvertToDisplay(computedResults, Context.User.Mention);
            await ReplyAsync(result);
        }

        public class ComputedResult
        {
            public int Roll { get; set; }
            public DiceTypeDto Die { get; set; }
        }

        public string ConvertToDisplay(IList<ComputedResult> computedResults, string user)
        {
            var rep = new StringBuilder();
            rep.AppendLine($":game_die: {user}");
            rep.Append("**Result:** ");
            var resultStrings = new List<string>();
            var total = 0;
            var extraText = "";
            foreach (var dice in computedResults)
            {
                var results = new List<int>();
                for (var i = 0; i < dice.Die.times; i++)
                {
                    var roll = dice.Roll;
                    total += roll;
                    results.Add(roll);
                    if (roll == dice.Die.sides)
                    {
                        extraText = "**Critical Hit!**";
                    }
                    if (roll == 1)
                    {
                        extraText = "**Critical Fail!**";
                    }
                }
                resultStrings.Add($"{dice.Die.diceType} ({string.Join(",", results)})");
            }
            rep.Append(string.Join(" + ", resultStrings));
            rep.AppendLine();
            rep.AppendLine($"**Total:** {total}");
            if (extraText != "")
            {
                rep.AppendLine(extraText);
            }
            return rep.ToString();
        }

        private int Compute(DiceTypeDto token)
        {
            return rnd.Next(1, token.sides);
        }

        public DiceTypeDto toComputation(string dice)
        {
            var diceParts = dice.Split('d');
            if (diceParts.Length == 2
                && int.TryParse(diceParts[0], out int times)
                && int.TryParse(diceParts[1], out int max))
            {
                if (max == 0 || times == 0)
                {
                    return null;
                }
                
                return new DiceTypeDto()
                {
                    diceType = dice,
                    times = times,
                    sides = max
                };
            } 
            if (dice.Length == 1
                && int.TryParse(diceParts[0], out int mod))
            {
                return new DiceTypeDto()
                {
                    mod = mod
                };
            }
            
            return null;
        }
    }

    public class DiceTypeDto
    {
        public string diceType { get; set; }
        public int times { get; set; }
        public int sides { get; set; }
        public int mod { get; set; }
    }
}