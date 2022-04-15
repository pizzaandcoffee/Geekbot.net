using System.Drawing;
using Geekbot.Core;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Interactions.Embed;
using Geekbot.Interactions.Resolved;
using Localization = Geekbot.Core.Localization;

namespace Geekbot.Commands.Karma;

public class Karma
{
    private readonly DatabaseContext _database;
    private readonly long _guildId;

    public Karma(DatabaseContext database, long guildId)
    {
        _database = database;
        _guildId = guildId;
    }

    public async Task<Embed> ChangeKarma(User author, User targetUser, KarmaChange change)
    {
        // Get the user
        var authorRecord = await GetUser(long.Parse(author.Id));

        // Check if the user can change karma
        if (targetUser.Id == author.Id)
        {
            var message = change switch
            {
                KarmaChange.Up => Localization.Karma.CannotChangeOwnUp,
                KarmaChange.Same => Localization.Karma.CannotChangeOwnSame,
                KarmaChange.Down => Localization.Karma.CannotChangeOwnDown,
                _ => throw new ArgumentOutOfRangeException(nameof(change), change, null)
            };
            return Embed.ErrorEmbed(string.Format(message, author.Username));
        }

        var timeoutMinutes = 3;
        if (authorRecord.TimeOut.AddMinutes(timeoutMinutes) > DateTimeOffset.Now.ToUniversalTime())
        {
            var remaining = authorRecord.TimeOut.AddMinutes(timeoutMinutes) - DateTimeOffset.Now.ToUniversalTime();
            var formatedWaitTime = DateLocalization.FormatDateTimeAsRemaining(remaining);
            return Embed.ErrorEmbed(string.Format(Localization.Karma.WaitUntill, author.Username, formatedWaitTime));
        }

        // Get the values for the change direction
        var (title, amount) = change switch
        {
            KarmaChange.Up => (Localization.Karma.Increased, 1),
            KarmaChange.Same => (Localization.Karma.Neutral, 0),
            KarmaChange.Down => (Localization.Karma.Decreased, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(change), change, null)
        };

        // Change it
        var targetUserRecord = await GetUser(long.Parse(targetUser.Id));
        targetUserRecord.Karma += amount;
        _database.Karma.Update(targetUserRecord);

        authorRecord.TimeOut = DateTimeOffset.Now.ToUniversalTime();
        _database.Karma.Update(authorRecord);

        await _database.SaveChangesAsync();

        // Respond
        var eb = new Embed()
        {
            Author = new ()
            {
                Name = targetUser.Username,
                IconUrl = targetUser.GetAvatarUrl()
            },
            Title = title,
        };
        eb.SetColor(Color.PaleGreen);
        eb.AddInlineField(Localization.Karma.By, author.Username);
        eb.AddInlineField(Localization.Karma.Amount, amount.ToString());
        eb.AddInlineField(Localization.Karma.Current, targetUserRecord.Karma.ToString());
        return eb;
    }

    private async Task<KarmaModel> GetUser(long userId)
    {
        var user = _database.Karma.FirstOrDefault(u => u.GuildId.Equals(_guildId) && u.UserId.Equals(userId)) ?? await CreateNewRow(userId);
        return user;
    }

    private async Task<KarmaModel> CreateNewRow(long userId)
    {
        var user = new KarmaModel()
        {
            GuildId = _guildId,
            UserId = userId,
            Karma = 0,
            TimeOut = DateTimeOffset.MinValue.ToUniversalTime()
        };
        var newUser = _database.Karma.Add(user).Entity;
        await _database.SaveChangesAsync();
        return newUser;
    }
}