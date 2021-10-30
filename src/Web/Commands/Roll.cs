using System;
using System.Threading.Tasks;
using Geekbot.Commands.Roll;
using Geekbot.Core.Database;
using Geekbot.Core.Interactions;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;
using Geekbot.Core.KvInMemoryStore;
using Geekbot.Core.RandomNumberGenerator;

namespace Geekbot.Web.Commands
{
    public class Roll : InteractionBase
    {
        private readonly IKvInMemoryStore _kvInMemoryStore;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public Roll(IKvInMemoryStore kvInMemoryStore, DatabaseContext database, IRandomNumberGenerator randomNumberGenerator)
        {
            _kvInMemoryStore = kvInMemoryStore;
            _randomNumberGenerator = randomNumberGenerator;
        }
        
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

            var number = _randomNumberGenerator.Next(1, 100);

            var replyContent = "";
            
            if (guess <= 100 && guess > 0)
            {
                var kvKey = $"{interaction.GuildId}:{interaction.Member.User.Id}:RollsPrevious";
                var prevRoll = _kvInMemoryStore.Get<RollTimeout>(kvKey);

                if (prevRoll?.LastGuess == guess && prevRoll?.GuessedOn.AddDays(1) > DateTime.Now)
                {
                    replyContent = string.Format(
                        ":red_circle: {0}, you can't guess the same number again, guess another number or wait {1}",
                        interaction.Member.Nick ?? interaction.Member.User.Username,
                        prevRoll.GuessedOn.AddDays(1));
                }
                else
                {
                    _kvInMemoryStore.Set(kvKey, new RollTimeout {LastGuess = guess, GuessedOn = DateTime.Now});
                    replyContent = $"{interaction.Member?.User?.Mention}, you rolled {number}, your guess was {guess}";
                }
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