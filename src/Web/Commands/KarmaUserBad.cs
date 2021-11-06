using Geekbot.Commands.Karma;
using Geekbot.Core.Database;
using Geekbot.Interactions;
using Geekbot.Interactions.ApplicationCommand;
using Geekbot.Interactions.Request;
using Geekbot.Interactions.Response;

namespace Geekbot.Web.Commands;

public class KarmaUserBad : InteractionBase
{
    private readonly DatabaseContext _database;

    public KarmaUserBad(DatabaseContext database)
    {
        _database = database;
    }
    
    public override Command GetCommandInfo() => new ()
        {
            Name = "Give bad karma",
            Type = CommandType.User,
        };

    public override async Task<InteractionResponse> Exec(Interaction interaction)
    {
        var author = interaction.Member.User;
        var targetUserId = interaction.Data.TargetId;
        var targetUser = interaction.Data.Resolved.Users[targetUserId];

        var karma = new Geekbot.Commands.Karma.Karma(_database, long.Parse(interaction.GuildId));
        var res = await karma.ChangeKarma(author, targetUser, KarmaChange.Down);
        
        return SimpleResponse(res);
    }
}