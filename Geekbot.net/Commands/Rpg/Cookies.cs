using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.CommandPreconditions;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Localization;
using Geekbot.net.Lib.RandomNumberGenerator;

namespace Geekbot.net.Commands.Rpg
{
    [DisableInDirectMessage]
    [Group("cookies")]
    public class Cookies : ModuleBase
    {
        private readonly DatabaseContext _database;
        private readonly IErrorHandler _errorHandler;
        private readonly ITranslationHandler _translation;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public Cookies(DatabaseContext database, IErrorHandler errorHandler, ITranslationHandler translation , IRandomNumberGenerator randomNumberGenerator)
        {
            _database = database;
            _errorHandler = errorHandler;
            _translation = translation;
            _randomNumberGenerator = randomNumberGenerator;
        }

        [Command("get", RunMode = RunMode.Async)]
        [Summary("Get a cookie every 24 hours")]
        public async Task GetCookies()
        {
            try
            {
                var transContext = await _translation.GetGuildContext(Context);
                var actor = await GetUser(Context.User.Id);
                if (actor.LastPayout.Value.AddHours(24) > DateTimeOffset.Now)
                {
                    var formatedWaitTime = transContext.FormatDateTimeAsRemaining(actor.LastPayout.Value.AddHours(24));
                    await ReplyAsync(transContext.GetString("WaitForMoreCookies", formatedWaitTime));
                    return;
                }
                actor.Cookies += 10;
                actor.LastPayout = DateTimeOffset.Now;
                await SetUser(actor);
                await ReplyAsync(transContext.GetString("GetCookies", 10, actor.Cookies));

            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("jar", RunMode = RunMode.Async)]
        [Summary("Look at your cookie jar")]
        public async Task PeekIntoCookieJar()
        {
            try
            {
                var transContext = await _translation.GetGuildContext(Context);
                var actor = await GetUser(Context.User.Id);
                await ReplyAsync(transContext.GetString("InYourJar", actor.Cookies));
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("give", RunMode = RunMode.Async)]
        [Summary("Give cookies to someone")]
        public async Task GiveACookie([Summary("User")] IUser user, [Summary("amount")] int amount)
        {
            try
            {
                var transContext = await _translation.GetGuildContext(Context);
                var giver = await GetUser(Context.User.Id);

                if (giver.Cookies < amount)
                {
                    await ReplyAsync(transContext.GetString("NotEnoughToGive"));
                    return;
                }

                var taker = await GetUser(user.Id);

                giver.Cookies -= amount;
                taker.Cookies += amount;
                
                await SetUser(giver);
                await SetUser(taker);
                
                await ReplyAsync(transContext.GetString("Given", amount, user.Username));
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("eat", RunMode = RunMode.Async)]
        [Summary("Eat a cookie")]
        public async Task EatACookie()
        {
            try
            {
                var transContext = await _translation.GetGuildContext(Context);
                var actor = await GetUser(Context.User.Id);

                if (actor.Cookies < 5)
                {
                    await ReplyAsync(transContext.GetString("NotEnoughCookiesToEat"));
                    return;
                }

                var amount = _randomNumberGenerator.Next(1, 5);
                actor.Cookies -= amount;
                
                await SetUser(actor);
                
                await ReplyAsync(transContext.GetString("AteCookies", amount, actor.Cookies));
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
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
                LastPayout = DateTimeOffset.MinValue
            };
            var newUser = _database.Cookies.Add(user).Entity;
            await _database.SaveChangesAsync();
            return newUser;
        }
    }
}
