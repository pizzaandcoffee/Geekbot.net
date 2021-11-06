using Geekbot.Commands.Karma;
using Geekbot.Core.Database;
using Geekbot.Core.Interactions;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;

namespace Geekbot.Web.Commands;

public class KarmaUserNeutral : InteractionBase
{
    private readonly DatabaseContext _database;

    public KarmaUserNeutral(DatabaseContext database)
    {
        _database = database;
    }

    public override Command GetCommandInfo() => new ()
    {
        Name = "Give neutral karma",
        Type = CommandType.User,
    };

    public override async Task<InteractionResponse> Exec(Interaction interaction)
    {
        var author = interaction.Member.User;
        var targetUserId = interaction.Data.TargetId;
        var targetUser = interaction.Data.Resolved.Users[targetUserId];

        var karma = new Geekbot.Commands.Karma.Karma(_database, long.Parse(interaction.GuildId));
        var res = await karma.ChangeKarma(author, targetUser, KarmaChange.Same);
        
        return SimpleResponse(res);
    }
}