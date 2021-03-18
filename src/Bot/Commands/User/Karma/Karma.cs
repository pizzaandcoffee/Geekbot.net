using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Bot.Utils;
using Geekbot.Core;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.GuildSettingsManager;

namespace Geekbot.Bot.Commands.User.Karma
{
    [DisableInDirectMessage]
    public class Karma : GeekbotCommandBase
    {
        private readonly DatabaseContext _database;

        public Karma(DatabaseContext database, IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
            _database = database;
        }

        [Command("good", RunMode = RunMode.Async)]
        [Summary("Increase Someones Karma")]
        public async Task Good([Summary("@someone")] IUser user)
        {
            await ChangeKarma(user, KarmaChange.Up);
        }

        [Command("bad", RunMode = RunMode.Async)]
        [Summary("Decrease Someones Karma")]
        public async Task Bad([Summary("@someone")] IUser user)
        {
            await ChangeKarma(user, KarmaChange.Down);
        }

        [Command("neutral", RunMode = RunMode.Async)]
        [Summary("Do nothing to someones Karma")]
        public async Task Neutral([Summary("@someone")] IUser user)
        {
            await ChangeKarma(user, KarmaChange.Same);
        }

        private async Task ChangeKarma(IUser user, KarmaChange change)
        {
            try
            {
                // Get the user
                var actor = await GetUser(Context.User.Id);
                
                // Check if the user can change karma
                if (user.Id == Context.User.Id)
                {
                    var message = change switch
                    {
                        KarmaChange.Up => Localization.Karma.CannotChangeOwnUp,
                        KarmaChange.Same => Localization.Karma.CannotChangeOwnSame,
                        KarmaChange.Down => Localization.Karma.CannotChangeOwnDown,
                        _ => throw new ArgumentOutOfRangeException(nameof(change), change, null)
                    };
                    await ReplyAsync(string.Format(message, Context.User.Username));
                    return;
                }

                if (actor.TimeOut.AddMinutes(3) > DateTimeOffset.Now)
                {
                    var formatedWaitTime = DateLocalization.FormatDateTimeAsRemaining(actor.TimeOut.AddMinutes(3));
                    await ReplyAsync(string.Format(Localization.Karma.WaitUntill, Context.User.Username, formatedWaitTime));
                    return;
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
                var target = await GetUser(user.Id);
                target.Karma += amount;
                _database.Karma.Update(target);

                actor.TimeOut = DateTimeOffset.Now;
                _database.Karma.Update(actor);

                await _database.SaveChangesAsync();

                // Respond
                var eb = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        Name = user.Username,
                        IconUrl = user.GetAvatarUrl()
                    },
                    Title = title,
                    Color = new Color(138, 219, 146)
                };
                eb.AddInlineField(Localization.Karma.By, Context.User.Username);
                eb.AddInlineField(Localization.Karma.Amount, amount.ToString());
                eb.AddInlineField(Localization.Karma.Current, target.Karma.ToString());
                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task<KarmaModel> GetUser(ulong userId)
        {
            var user = _database.Karma.FirstOrDefault(u => u.GuildId.Equals(Context.Guild.Id.AsLong()) && u.UserId.Equals(userId.AsLong())) ?? await CreateNewRow(userId);
            return user;
        }

        private async Task<KarmaModel> CreateNewRow(ulong userId)
        {
            var user = new KarmaModel()
            {
                GuildId = Context.Guild.Id.AsLong(),
                UserId = userId.AsLong(),
                Karma = 0,
                TimeOut = DateTimeOffset.MinValue
            };
            var newUser = _database.Karma.Add(user).Entity;
            await _database.SaveChangesAsync();
            return newUser;
        }
    }
}