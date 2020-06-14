using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.KvInMemoryStore;
using Geekbot.net.Lib.Localization;
using Geekbot.net.Lib.RandomNumberGenerator;

namespace Geekbot.net.Commands.Games.Roll
{
    public class Roll : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IKvInMemoryStore _kvInMemoryStore;
        private readonly ITranslationHandler _translation;
        private readonly DatabaseContext _database;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public Roll(IKvInMemoryStore kvInMemoryStore,IErrorHandler errorHandler, ITranslationHandler translation, DatabaseContext database, IRandomNumberGenerator randomNumberGenerator)
        {
            _kvInMemoryStore = kvInMemoryStore;
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
                int.TryParse(stuff, out var guess);
                var transContext = await _translation.GetGuildContext(Context);
                if (guess <= 100 && guess > 0)
                {
                    var kvKey = $"{Context.Guild.Id}:{Context.User.Id}:RollsPrevious";

                    var prevRoll = _kvInMemoryStore.Get<RollTimeout>(kvKey);

                    if (prevRoll?.LastGuess == guess && prevRoll?.GuessedOn.AddDays(1) > DateTime.Now)
                    {
                        await ReplyAsync(transContext.GetString(
                            "NoPrevGuess",
                            Context.Message.Author.Mention,
                            transContext.FormatDateTimeAsRemaining(prevRoll.GuessedOn.AddDays(1))));
                        return;
                    }

                    _kvInMemoryStore.Set(kvKey, new RollTimeout { LastGuess = guess, GuessedOn = DateTime.Now });

                    await ReplyAsync(transContext.GetString("Rolled", Context.Message.Author.Mention, number, guess));
                    if (guess == number)
                    {
                        await ReplyAsync(transContext.GetString("Gratz", Context.Message.Author));
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