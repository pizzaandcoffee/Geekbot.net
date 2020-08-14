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

namespace Geekbot.Bot.Commands.User
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
            try
            {
                var actor = await GetUser(Context.User.Id);
                if (user.Id == Context.User.Id)
                {
                    await ReplyAsync(string.Format(Localization.Karma.CannotChangeOwnUp, Context.User.Username));
                }
                else if (TimeoutFinished(actor.TimeOut))
                {
                    var formatedWaitTime = DateLocalization.FormatDateTimeAsRemaining(actor.TimeOut.AddMinutes(3));
                    await ReplyAsync(string.Format(Localization.Karma.WaitUntill, Context.User.Username, formatedWaitTime));
                }
                else
                {
                    var target = await GetUser(user.Id);
                    target.Karma += 1;
                    SetUser(target);
                    
                    actor.TimeOut = DateTimeOffset.Now;
                    SetUser(actor);

                    await _database.SaveChangesAsync();

                    var eb = new EmbedBuilder();
                    eb.WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(user.Username));

                    eb.WithColor(new Color(138, 219, 146));
                    eb.Title = Localization.Karma.Increased;
                    eb.AddInlineField(Localization.Karma.By, Context.User.Username);
                    eb.AddInlineField(Localization.Karma.Amount, "+1");
                    eb.AddInlineField(Localization.Karma.Current, target.Karma);
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("bad", RunMode = RunMode.Async)]
        [Summary("Decrease Someones Karma")]
        public async Task Bad([Summary("@someone")] IUser user)
        {
            try
            {
                var actor = await GetUser(Context.User.Id);
                if (user.Id == Context.User.Id)
                {
                    await ReplyAsync(string.Format(Localization.Karma.CannotChangeOwnDown, Context.User.Username));
                }
                else if (TimeoutFinished(actor.TimeOut))
                {
                    var formatedWaitTime = DateLocalization.FormatDateTimeAsRemaining(actor.TimeOut.AddMinutes(3));
                    await ReplyAsync(string.Format(Localization.Karma.WaitUntill, Context.User.Username, formatedWaitTime));
                }
                else
                {
                    var target = await GetUser(user.Id);
                    target.Karma -= 1;
                    SetUser(target);
                    
                    actor.TimeOut = DateTimeOffset.Now;
                    SetUser(actor);

                    await _database.SaveChangesAsync();

                    var eb = new EmbedBuilder();
                    eb.WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(user.Username));

                    eb.WithColor(new Color(138, 219, 146));
                    eb.Title = Localization.Karma.Decreased;
                    eb.AddInlineField(Localization.Karma.By, Context.User.Username);
                    eb.AddInlineField(Localization.Karma.Amount, "-1");
                    eb.AddInlineField(Localization.Karma.Current, target.Karma);
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        private bool TimeoutFinished(DateTimeOffset lastKarma)
        {
            return lastKarma.AddMinutes(3) > DateTimeOffset.Now;
        }

        private async Task<KarmaModel> GetUser(ulong userId)
        {
            var user = _database.Karma.FirstOrDefault(u =>u.GuildId.Equals(Context.Guild.Id.AsLong()) && u.UserId.Equals(userId.AsLong())) ?? await CreateNewRow(userId);
            return user;
        }
        
        private void SetUser(KarmaModel user)
        {
            _database.Karma.Update(user);
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