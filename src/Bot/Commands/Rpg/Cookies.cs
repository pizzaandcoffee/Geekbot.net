using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Bot.CommandPreconditions;
using Geekbot.Core;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.RandomNumberGenerator;
using Localization = Geekbot.Core.Localization;

namespace Geekbot.Bot.Commands.Rpg
{
    [DisableInDirectMessage]
    [Group("cookies")]
    [Alias("cookie")]
    public class Cookies : GeekbotCommandBase
    {
        private readonly DatabaseContext _database;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public Cookies(DatabaseContext database, IErrorHandler errorHandler, IRandomNumberGenerator randomNumberGenerator, IGuildSettingsManager guildSettingsManager)
            : base(errorHandler, guildSettingsManager)
        {
            _database = database;
            _randomNumberGenerator = randomNumberGenerator;
        }

        [Command("get", RunMode = RunMode.Async)]
        [Summary("Get a cookie every 24 hours")]
        public async Task GetCookies()
        {
            try
            {
                var actor = await GetUser(Context.User.Id);
                var timeoutDays = 1;
                if (actor.LastPayout?.AddDays(timeoutDays) > DateTime.Now.ToUniversalTime())
                {
                    var remaining = actor.LastPayout.Value.AddDays(timeoutDays) - DateTimeOffset.Now.ToUniversalTime();
                    var formattedWaitTime = DateLocalization.FormatDateTimeAsRemaining(remaining);
                    await ReplyAsync(string.Format(Localization.Cookies.WaitForMoreCookies, formattedWaitTime));
                    return;
                }
                actor.Cookies += 10;
                actor.LastPayout = DateTimeOffset.Now.ToUniversalTime();
                await SetUser(actor);
                await ReplyAsync(string.Format(Localization.Cookies.GetCookies, 10, actor.Cookies));

            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("jar", RunMode = RunMode.Async)]
        [Summary("Look at your cookie jar")]
        public async Task PeekIntoCookieJar()
        {
            try
            {
                var actor = await GetUser(Context.User.Id);
                await ReplyAsync(string.Format(Localization.Cookies.InYourJar, actor.Cookies));
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("give", RunMode = RunMode.Async)]
        [Summary("Give cookies to someone")]
        public async Task GiveACookie([Summary("@someone")] IUser user, [Summary("amount")] int amount = 1)
        {
            try
            {
                var giver = await GetUser(Context.User.Id);

                if (giver.Cookies < amount)
                {
                    await ReplyAsync(Localization.Cookies.NotEnoughToGive);
                    return;
                }

                var taker = await GetUser(user.Id);

                giver.Cookies -= amount;
                taker.Cookies += amount;
                
                await SetUser(giver);
                await SetUser(taker);
                
                await ReplyAsync(string.Format(Localization.Cookies.Given, amount, user.Username));
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("eat", RunMode = RunMode.Async)]
        [Summary("Eat a cookie")]
        public async Task EatACookie()
        {
            try
            {
                var actor = await GetUser(Context.User.Id);

                if (actor.Cookies < 5)
                {
                    await ReplyAsync(Localization.Cookies.NotEnoughCookiesToEat);
                    return;
                }

                var amount = _randomNumberGenerator.Next(1, 5);
                actor.Cookies -= amount;
                
                await SetUser(actor);
                
                await ReplyAsync(string.Format(Localization.Cookies.AteCookies, amount, actor.Cookies));
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task<CookiesModel> GetUser(ulong userId)
        {
            var user = _database.Cookies.FirstOrDefault(u =>u.GuildId.Equals(Context.Guild.Id.AsLong()) && u.UserId.Equals(userId.AsLong())) ?? await CreateNewRow(userId);
            return user;
        }
        
        private async Task SetUser(CookiesModel user)
        {
            _database.Cookies.Update(user);
            await _database.SaveChangesAsync();
        }
        
        private async Task<CookiesModel> CreateNewRow(ulong userId)
        {
            var user = new CookiesModel()
            {
                GuildId = Context.Guild.Id.AsLong(),
                UserId = userId.AsLong(),
                Cookies = 0,
                LastPayout = DateTimeOffset.MinValue.ToUniversalTime()
            };
            var newUser = _database.Cookies.Add(user).Entity;
            await _database.SaveChangesAsync();
            return newUser;
        }
    }
}
