using System.Threading.Tasks;
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
        private readonly DatabaseContext _database;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public Roll(IKvInMemoryStore kvInMemoryStore, DatabaseContext database, IRandomNumberGenerator randomNumberGenerator)
        {
            _kvInMemoryStore = kvInMemoryStore;
            _database = database;
            _randomNumberGenerator = randomNumberGenerator;
        }
        
        private enum Options
        {
            Guess
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
                        Name = Options.Guess.ToString().ToLower(),
                        Description = "A number between 1 and 100 (inclusive)",
                        Required = true,
                        Type = OptionType.Integer
                    }
                }
            };
        }

        public override async Task<InteractionResponse> Exec(Interaction interaction)
        {
            var guessOption = interaction.Data.Options.Find(o => o.Name == Options.Guess.ToString().ToLower());
            var guess = guessOption.Value.GetInt32();
            
            var res = await new Geekbot.Commands.Roll.Roll(_kvInMemoryStore, _database, _randomNumberGenerator)
                .RunFromInteraction(
                    interaction.GuildId,
                    interaction.Member.User.Id,
                    interaction.Member.Nick ?? interaction.Member.User.Username,
                    guess
                );
            
            return new InteractionResponse()
            {
                Type = InteractionResponseType.ChannelMessageWithSource,
                Data = new InteractionResponseData()
                {
                    Content = res
                }
            };
        }
    }
}