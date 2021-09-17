using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.DiceParser;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Utils
{
    public class Dice : TransactionModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDiceParser _diceParser;

        public Dice(IErrorHandler errorHandler, IDiceParser diceParser)
        {
            _errorHandler = errorHandler;
            _diceParser = diceParser;
        }

        // ToDo: Separate representation and logic
        // ToDo: Translate
        [Command("dice", RunMode = RunMode.Async)]
        [Summary("Roll a dice. (use '!dice help' for a manual)")]
        public async Task RollCommand([Remainder] [Summary("input")] string diceInput = "1d20")
        {
            try
            {
                if (diceInput == "help")
                {
                    await ShowDiceHelp();
                    return;
                }
                
                var parsed = _diceParser.Parse(diceInput);

                var sb = new StringBuilder();
                sb.AppendLine($"{Context.User.Mention} :game_die:");
                foreach (var die in parsed.Dice)
                {
                    sb.AppendLine($"**{die.DiceName}**");
                    var diceResultList = new List<string>();
                    var total = 0;

                    foreach (var roll in die.Roll())
                    {
                        diceResultList.Add(roll.ToString());
                        total += roll.Result;
                    }
                    
                    sb.AppendLine(string.Join(" | ", diceResultList));

                    if (parsed.SkillModifier != 0)
                    {
                        sb.AppendLine($"Skill: {parsed.SkillModifier}");
                    }

                    if (parsed.Options.ShowTotal)
                    {
                        var totalLine = $"Total: {total}";
                        if (parsed.SkillModifier > 0)
                        {
                            totalLine += ($" (+{parsed.SkillModifier} = {total + parsed.SkillModifier})");
                        }

                        if (parsed.SkillModifier < 0)
                        {
                            totalLine += ($" ({parsed.SkillModifier} = {total - parsed.SkillModifier})");
                        }

                        sb.AppendLine(totalLine);
                    }
                }

                await Context.Channel.SendMessageAsync(sb.ToString());
            }
            catch (DiceException e)
            {
                await Context.Channel.SendMessageAsync($"**:warning: {e.DiceName} is invalid:** {e.Message}");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task ShowDiceHelp()
        {
            var sb = new StringBuilder();
            sb.AppendLine("**__Examples__**");
            sb.AppendLine("```");
            sb.AppendLine("!dice               - throw a 1d20");
            sb.AppendLine("!dice 1d12          - throw a 1d12");
            sb.AppendLine("!dice +1d20         - throw with advantage");
            sb.AppendLine("!dice -1d20         - throw with disadvantage");
            sb.AppendLine("!dice 1d20 +2       - throw with a +2 skill bonus");
            sb.AppendLine("!dice 1d20 -2       - throw with a -2 skill bonus");
            sb.AppendLine("!dice 8d6           - throw a fireball 🔥");
            sb.AppendLine("!dice 8d6 total     - calculate the total");
            sb.AppendLine("!dice 2d20 6d6 2d12 - drop your dice pouch");
            sb.AppendLine("```");
            
            await Context.Channel.SendMessageAsync(sb.ToString());
        }
    }
}