using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Localization;

namespace Geekbot.net.Commands.User
{
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
        [Remarks(CommandCategories.Karma)]
        [Summary("Increase Someones Karma")]
        public async Task Good([Summary("@someone")] IUser user)
        {
            try
            {
                var transDict = _translation.GetDict(Context);
                var actor = GetUser(Context.User.Id);
                if (user.Id == Context.User.Id)
                {
                    await ReplyAsync(string.Format(transDict["CannotChangeOwn"], Context.User.Username));
                }
                else if (TimeoutFinished(actor.TimeOut))
                {
                    await ReplyAsync(string.Format(transDict["WaitUntill"], Context.User.Username,
                        GetTimeLeft(actor.TimeOut)));
                }
                else
                {
                    var target = GetUser(user.Id);
                    target.Karma = target.Karma + 1;
                    SetUser(target);
                    
                    actor.TimeOut = DateTimeOffset.Now;
                    SetUser(actor);

                    _database.SaveChanges();

                    var eb = new EmbedBuilder();
                    eb.WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(user.Username));

                    eb.WithColor(new Color(138, 219, 146));
                    eb.Title = transDict["Increased"];
                    eb.AddInlineField(transDict["By"], Context.User.Username);
                    eb.AddInlineField(transDict["Amount"], "+1");
                    eb.AddInlineField(transDict["Current"], target.Karma);
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("bad", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Karma)]
        [Summary("Decrease Someones Karma")]
        public async Task Bad([Summary("@someone")] IUser user)
        {
            try
            {
                var transDict = _translation.GetDict(Context);
                var actor = GetUser(Context.User.Id);
                if (user.Id == Context.User.Id)
                {
                    await ReplyAsync(string.Format(transDict["CannotChangeOwn"], Context.User.Username));
                }
                else if (TimeoutFinished(actor.TimeOut))
                {
                    await ReplyAsync(string.Format(transDict["WaitUntill"], Context.User.Username,
                        GetTimeLeft(actor.TimeOut)));
                }
                else
                {
                    var target = GetUser(user.Id);
                    target.Karma = target.Karma - 1;
                    SetUser(target);
                    
                    actor.TimeOut = DateTimeOffset.Now;
                    SetUser(actor);

                    _database.SaveChanges();

                    var eb = new EmbedBuilder();
                    eb.WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(user.Username));

                    eb.WithColor(new Color(138, 219, 146));
                    eb.Title = transDict["Decreased"];
                    eb.AddInlineField(transDict["By"], Context.User.Username);
                    eb.AddInlineField(transDict["Amount"], "-1");
                    eb.AddInlineField(transDict["Current"], target.Karma);
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private bool TimeoutFinished(DateTimeOffset lastKarma)
        {
            return lastKarma.AddMinutes(3) > DateTimeOffset.Now;
        }

        private string GetTimeLeft(DateTimeOffset lastKarma)
        {
            var dt = lastKarma.AddMinutes(3).Subtract(DateTimeOffset.Now);
            return $"{dt.Minutes} Minutes and {dt.Seconds} Seconds";
        }

        private KarmaModel GetUser(ulong userId)
        {
            var user = _database.Karma.FirstOrDefault(u =>u.GuildId.Equals(Context.Guild.Id.AsLong()) && u.UserId.Equals(userId.AsLong())) ?? CreateNewRow(userId);
            return user;
        }
        
        private bool SetUser(KarmaModel user)
        {
            _database.Karma.Update(user);
            return true;
        }
        
        private KarmaModel CreateNewRow(ulong userId)
        {
            var user = new KarmaModel()
            {
                GuildId = Context.Guild.Id.AsLong(),
                UserId = userId.AsLong(),
                Karma = 0,
                TimeOut = DateTimeOffset.MinValue
            };
            var newUser = _database.Karma.Add(user).Entity;
            _database.SaveChanges();
            return newUser;
        }
    }
}