using Geekbot.Core;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.Extensions;
using Geekbot.Core.KvInMemoryStore;
using Geekbot.Core.RandomNumberGenerator;

namespace Geekbot.Commands.Roll
{
    public class Roll
    {
        private readonly IKvInMemoryStore _kvInMemoryStore;
        private readonly DatabaseContext _database;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public Roll(IKvInMemoryStore kvInMemoryStore, DatabaseContext database, IRandomNumberGenerator randomNumberGenerator)
        {
            _kvInMemoryStore = kvInMemoryStore;
            _database = database;
            _randomNumberGenerator = randomNumberGenerator;
        }

        public async Task<string> RunFromGateway(ulong guildId, ulong userId, string userName, string unparsedGuess)
        {
            int.TryParse(unparsedGuess, out var guess);
            return await this.Run(guildId.AsLong(), userId.AsLong(), userName, guess);
        }

        public async Task<string> RunFromInteraction(string guildId, string userId, string userName, int guess)
        {
            return await this.Run(long.Parse(guildId), long.Parse(userId), userName, guess);
        }
        
        private async Task<string> Run(long guildId, long userId, string userName, int guess)
        {
            var number = _randomNumberGenerator.Next(1, 100);

            if (guess <= 100 && guess > 0)
            {
                var kvKey = $"{guildId}:{userId}:RollsPrevious";
                var prevRoll = _kvInMemoryStore.Get<RollTimeout>(kvKey);

                if (prevRoll?.LastGuess == guess && prevRoll?.GuessedOn.AddDays(1) > DateTime.Now)
                {
                    return string.Format(
                        Core.Localization.Roll.NoPrevGuess,
                        $"<@{userId}>",
                        DateLocalization.FormatDateTimeAsRemaining(prevRoll.GuessedOn.AddDays(1)));
                }

                _kvInMemoryStore.Set(kvKey, new RollTimeout { LastGuess = guess, GuessedOn = DateTime.Now });

                var answer = string.Format(Core.Localization.Roll.Rolled, $"<@{userId}>", number, guess);
                
                if (guess == number)
                {
                    var user = await GetUser(guildId, userId);
                    user.Rolls += 1;
                    _database.Rolls.Update(user);
                    await _database.SaveChangesAsync();
                    answer += string.Format(($"\n{Core.Localization.Roll.Gratz}"), userName);
                }
                
                return answer;
            }
            else
            {
                return string.Format(Core.Localization.Roll.RolledNoGuess, $"<@{userId}>", number);
            }
        }
        
        private async Task<RollsModel> GetUser(long guildId, long userId)
        {
            var user = _database.Rolls.FirstOrDefault(u => u.GuildId.Equals(guildId) && u.UserId.Equals(userId)) ?? await CreateNewRow(guildId, userId);
            return user;
        }

        private async Task<RollsModel> CreateNewRow(long guildId, long userId)
        {
            var user = new RollsModel()
            {
                GuildId = guildId,
                UserId = userId,
                Rolls = 0
            };
            var newUser = _database.Rolls.Add(user).Entity;
            await _database.SaveChangesAsync();
            return newUser;
        }
    }
}