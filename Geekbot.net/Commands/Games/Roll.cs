using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.AlmostRedis;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Localization;
using Geekbot.net.Lib.RandomNumberGenerator;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Games
{
    public class Roll : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IAlmostRedis _redis;
        private readonly ITranslationHandler _translation;
        private readonly DatabaseContext _database;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public Roll(IAlmostRedis redis, IErrorHandler errorHandler, ITranslationHandler translation, DatabaseContext database, IRandomNumberGenerator randomNumberGenerator)
        {
            _redis = redis;
            _translation = translation;
            _database = database;
            _randomNumberGenerator = randomNumberGenerator;
            _errorHandler = errorHandler;
        }

        [Command("roll", RunMode = RunMode.Async)]
        [Summary("Guess which number the bot will roll (1-100")]
        public async Task RollCommand([Remainder] [Summary("guess")] string stuff = null)
        {
            try
            {
                var number = _randomNumberGenerator.Next(1, 100);
                var guess = 1000;
                int.TryParse(stuff, out guess);
                var transContext = await _translation.GetGuildContext(Context);
                if (guess <= 100 && guess > 0)
                {
                    var prevRoll = _redis.Db.HashGet($"{Context.Guild.Id}:RollsPrevious2", Context.Message.Author.Id).ToString()?.Split('|');
                    if (prevRoll?.Length == 2)
                    {
                        if (prevRoll[0] == guess.ToString() && DateTime.Parse(prevRoll[1]) > DateTime.Now.AddDays(-1))
                        {
                            await ReplyAsync(transContext.GetString("NoPrevGuess", Context.Message.Author.Mention));
                            return;
                        }
                    }

                    _redis.Db.HashSet($"{Context.Guild.Id}:RollsPrevious2", new[] {new HashEntry(Context.Message.Author.Id, $"{guess}|{DateTime.Now}")});

                    await ReplyAsync(transContext.GetString("Rolled", Context.Message.Author.Mention, number, guess));
                    if (guess == number)
                    {
                        await ReplyAsync(transContext.GetString("Gratz", Context.Message.Author));
                        _redis.Db.HashIncrement($"{Context.Guild.Id}:Rolls", Context.User.Id.ToString());
                        var user = await GetUser(Context.User.Id);
                        user.Rolls += 1;
                        _database.Rolls.Update(user);
                        await _database.SaveChangesAsync();
                    }
                }
                else
                {
                    await ReplyAsync(transContext.GetString("RolledNoGuess", Context.Message.Author.Mention, number));
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        private async Task<RollsModel> GetUser(ulong userId)
        {
            var user = _database.Rolls.FirstOrDefault(u =>u.GuildId.Equals(Context.Guild.Id.AsLong()) && u.UserId.Equals(userId.AsLong())) ?? await CreateNewRow(userId);
            return user;
        }
        
        private async Task<RollsModel> CreateNewRow(ulong userId)
        {
            var user = new RollsModel()
            {
                GuildId = Context.Guild.Id.AsLong(),
                UserId = userId.AsLong(),
                Rolls = 0
            };
            var newUser = _database.Rolls.Add(user).Entity;
            await _database.SaveChangesAsync();
            return newUser;
        }
    }
}