using System.Collections.Generic;
using System.Threading.Tasks;
using Geekbot.Core.Interactions;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Resolved;
using Geekbot.Core.Interactions.Response;
using Geekbot.Core.RandomNumberGenerator;

namespace Geekbot.Web.Commands
{
    public class Roll : InteractionBase
    {
        public override Command GetCommandInfo()
        {
            return new Command()
            {
                Name = "roll",
                Description = "BETA: Roll and see if you can guess the correct number",
                Type = CommandType.ChatInput,
                Options = new()
                {
                    new Option()
                    {
                        Name = "guess",
                        Description = "A number between 1 and 100 (inclusive)",
                        Required = true,
                        Type = OptionType.Integer
                    }
                }
            };
        }

        public override async Task<InteractionResponse> Exec(Interaction interaction)
        {
            var guessOption = interaction.Data.Options.Find(o => o.Name == "guess");
            var guess = guessOption.Value.GetInt32();

            var number = new RandomNumberGenerator().Next(1, 100);

            var replyContent = "";
            
            if (guess <= 100 && guess > 0)
            {
                replyContent = $"{interaction?.Member?.User?.Mention}, you rolled {number}, your guess was {guess}";
            }
            else
            {
                replyContent = $"{interaction?.Member?.User?.Mention}, you rolled {number}";
            }
            
            return new InteractionResponse()
            {
                Type = InteractionResponseType.ChannelMessageWithSource,
                Data = new InteractionResponseData()
                {
                    Content = replyContent
                }
            };
        }
    }
}