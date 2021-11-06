using Geekbot.Commands.Karma;
using Geekbot.Core.Database;
using Geekbot.Core.Interactions;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;

namespace Geekbot.Web.Commands;

public class Karma : InteractionBase
{
    private readonly DatabaseContext _database;
    
    public Karma(DatabaseContext database)
    {
        _database = database;
    }
    
    private struct Options
    {
        internal const string Type = "type";
        internal const string Good = "good";
        internal const string Bad = "bad";
        internal const string Neutral = "neutral";
        internal const string User = "user";
    }
    
    public override Command GetCommandInfo() => new ()
        {
            Name = "karma",
            Description = "Interact with someones karma",
            Type = CommandType.ChatInput,
            Options = new()
            {
                new()
                {
                    Name = Options.Type,
                    Description = "Give someone karma",
                    Type = OptionType.String,
                    Required = true,
                    Choices = new[] { Options.Good, Options.Neutral, Options.Bad }
                        .Select(s => new OptionChoice() { Name = s, Value = s }).ToList()
                },
                new ()
                {
                    Name = Options.User,
                    Description = "The User",
                    Type = OptionType.User,
                    Required = true,
                }
            }
        };

    public override async Task<InteractionResponse> Exec(Interaction interaction)
    {
        var karmaType = interaction.Data.Options.Find(o => o.Name == Options.Type);
        var targetUserId = interaction.Data.Options.Find(o => o.Name == Options.User);

        var karmaChange = karmaType.Value.GetString() switch
        {
            Options.Good => KarmaChange.Up,
            Options.Neutral => KarmaChange.Same,
            Options.Bad => KarmaChange.Down,
            _ => throw new ArgumentOutOfRangeException()
        };

        var author = interaction.Member.User;
        var targetUser = interaction.Data.Resolved.Users[targetUserId.Value.GetString()];
        
        var karma = new Geekbot.Commands.Karma.Karma(_database, long.Parse(interaction.GuildId));
        var res = await karma.ChangeKarma(author, targetUser, karmaChange);

        return SimpleResponse(res);
    }
}