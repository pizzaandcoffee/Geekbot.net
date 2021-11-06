using Geekbot.Core.Converters;
using Geekbot.Core.Database;
using Geekbot.Core.Highscores;
using Geekbot.Core.Interactions;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;

namespace Geekbot.Web.Commands;

public class Rank : InteractionBase
{
    private readonly DatabaseContext _database;
    private readonly IEmojiConverter _emojiConverter;
    private readonly IHighscoreManager _highscoreManager;

    public Rank(DatabaseContext database, IEmojiConverter emojiConverter, IHighscoreManager highscoreManager)
    {
        _database = database;
        _emojiConverter = emojiConverter;
        _highscoreManager = highscoreManager;
    }

    private struct Options
    {
        internal const string Counter = "counter";
        internal const string Amount = "amount";
        internal const string Season = "season";
    }
    
    public override Command GetCommandInfo()
    {
        return new Command()
        {
            Name = "rank",
            Description = "BETA: Highscores for various counters",
            Type = CommandType.ChatInput,
            Options = new List<Option>()
            {
                new ()
                {
                    Name = Options.Counter,
                    Description = "The counter to show",
                    Required = true,
                    Type = OptionType.String,
                    Choices = Enumerable.Select(
                        Enum.GetNames<HighscoreTypes>(),
                        highscoreType => new OptionChoice()
                        {
                            Name = highscoreType,
                            Value = highscoreType
                        }).ToList()
                },
                new ()
                {
                    Name = Options.Amount,
                    Description = "Amount of positions to show in the list",
                    Required = false,
                    Type = OptionType.Integer
                },
                new ()
                {
                    Name = Options.Season,
                    Description = "Select the season, only applies for the seasons counter",
                    Required = false,
                    Type = OptionType.String
                }
            }
        };
    }

    public override Task<InteractionResponse> Exec(Interaction interaction)
    {
        var counterTypeOption = interaction.Data.Options.Find(o => o.Name == Options.Counter);
        var amountOption = interaction.Data.Options.Find(o => o.Name == Options.Amount);
        var seasonOption = interaction.Data.Options.Find(o => o.Name == Options.Season);
        
        var res = new Geekbot.Commands.Rank(_database, _emojiConverter, _highscoreManager)
            .Run(
                counterTypeOption?.Value.GetString() ?? HighscoreTypes.messages.ToString(),
                amountOption?.Value.GetInt32() ?? 10,
                seasonOption?.Value.GetString() ?? string.Empty,
                ulong.Parse(interaction.GuildId),
                "...");
        
        var interactionResponse = new InteractionResponse()
        {
            Type = InteractionResponseType.ChannelMessageWithSource,
            Data = new InteractionResponseData()
            {
                Content = res
            }
        };

        return Task.FromResult(SimpleResponse(res));
    }
}
