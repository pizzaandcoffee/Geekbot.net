using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.Localization;

namespace Geekbot.Bot.Commands.User
{
    [DisableInDirectMessage]
    public class Karma : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly DatabaseContext _database;
        private readonly ITranslationHandler _translation;

        public Karma(DatabaseContext database, IErrorHandler errorHandler, ITranslationHandler translation)
        {
            _database = database;
            _errorHandler = errorHandler;
            _translation = translation;
        }

        [Command("good", RunMode = RunMode.Async)]
        [Summary("Increase Someones Karma")]
        public async Task Good([Summary("@someone")] IUser user)
        {
            try
            {
                var transContext = await _translation.GetGuildContext(Context);
                var actor = await GetUser(Context.User.Id);
                if (user.Id == Context.User.Id)
                {
                    await ReplyAsync(transContext.GetString("CannotChangeOwn", Context.User.Username));
                }
                else if (TimeoutFinished(actor.TimeOut))
                {
                    var formatedWaitTime = transContext.FormatDateTimeAsRemaining(actor.TimeOut.AddMinutes(3));
                    await ReplyAsync(transContext.GetString("WaitUntill", Context.User.Username, formatedWaitTime));
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
                    eb.Title = transContext.GetString("Increased");
                    eb.AddInlineField(transContext.GetString("By"), Context.User.Username);
                    eb.AddInlineField(transContext.GetString("Amount"), "+1");
                    eb.AddInlineField(transContext.GetString("Current"), target.Karma);
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("bad", RunMode = RunMode.Async)]
        [Summary("Decrease Someones Karma")]
        public async Task Bad([Summary("@someone")] IUser user)
        {
            try
            {
                var transContext = await _translation.GetGuildContext(Context);
                var actor = await GetUser(Context.User.Id);
                if (user.Id == Context.User.Id)
                {
                    await ReplyAsync(transContext.GetString("CannotChangeOwn", Context.User.Username));
                }
                else if (TimeoutFinished(actor.TimeOut))
                {
                    var formatedWaitTime = transContext.FormatDateTimeAsRemaining(actor.TimeOut.AddMinutes(3));
                    await ReplyAsync(transContext.GetString("WaitUntill", Context.User.Username, formatedWaitTime));
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
                    eb.Title = transContext.GetString("Decreased");
                    eb.AddInlineField(transContext.GetString("By"), Context.User.Username);
                    eb.AddInlineField(transContext.GetString("Amount"), "-1");
                    eb.AddInlineField(transContext.GetString("Current"), target.Karma);
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
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