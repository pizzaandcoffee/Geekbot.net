using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Bot.Utils;
using Geekbot.Core;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.KvInMemoryStore;
using Geekbot.Core.RandomNumberGenerator;

namespace Geekbot.Bot.Commands.Games.Roll
{
    public class Roll : GeekbotCommandBase
    {
        private readonly IKvInMemoryStore _kvInMemoryStore;
        private readonly DatabaseContext _database;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public Roll(IKvInMemoryStore kvInMemoryStore, IErrorHandler errorHandler, DatabaseContext database, IRandomNumberGenerator randomNumberGenerator, IGuildSettingsManager guildSettingsManager)
            : base(errorHandler, guildSettingsManager)
        {
            _kvInMemoryStore = kvInMemoryStore;
            _database = database;
            _randomNumberGenerator = randomNumberGenerator;
        }

        [Command("roll", RunMode = RunMode.Async)]
        [Summary("Guess which number the bot will roll (1-100")]
        public async Task RollCommand([Remainder] [Summary("guess")] string stuff = null)
        {
            try
            {
                var number = _randomNumberGenerator.Next(1, 100);
                int.TryParse(stuff, out var guess);
                if (guess <= 100 && guess > 0)
                {
                    var kvKey = $"{Context?.Guild?.Id ?? 0}:{Context.User.Id}:RollsPrevious";

                    var prevRoll = _kvInMemoryStore.Get<RollTimeout>(kvKey);

                    if (prevRoll?.LastGuess == guess && prevRoll?.GuessedOn.AddDays(1) > DateTime.Now)
                    {
                        await ReplyAsync(string.Format(
                            Localization.Roll.NoPrevGuess,
                            Context.Message.Author.Mention,
                            DateLocalization.FormatDateTimeAsRemaining(prevRoll.GuessedOn.AddDays(1))));
                        return;
                    }

                    _kvInMemoryStore.Set(kvKey, new RollTimeout {LastGuess = guess, GuessedOn = DateTime.Now});

                    await ReplyAsync(string.Format(Localization.Roll.Rolled, Context.Message.Author.Mention, number, guess));
                    if (guess == number)
                    {
                        await ReplyAsync(string.Format(Localization.Roll.Gratz, Context.Message.Author));
                        var user = await GetUser(Context.User.Id);
                        user.Rolls += 1;
                        _database.Rolls.Update(user);
                        await _database.SaveChangesAsync();
                    }
                }
                else
                {
                    await ReplyAsync(string.Format(Localization.Roll.RolledNoGuess, Context.Message.Author.Mention, number));
                }
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task<RollsModel> GetUser(ulong userId)
        {
            var user = _database.Rolls.FirstOrDefault(u => u.GuildId.Equals(Context.Guild.Id.AsLong()) && u.UserId.Equals(userId.AsLong())) ?? await CreateNewRow(userId);
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