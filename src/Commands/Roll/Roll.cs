using System;
using System.Linq;
using System.Threading.Tasks;
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
            return await this.Run(guildId, userId, userName, guess);
        }

        public async Task<string> RunFromInteraction(string guildId, string userId, string userName, int guess)
        {
            
            return await this.Run(ulong.Parse(guildId), ulong.Parse(userId), userName, guess);
        }
        
        private async Task<string> Run(ulong guildId, ulong userId, string userName, int guess)
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
        
        private async Task<RollsModel> GetUser(ulong guildId, ulong userId)
        {
            var user = _database.Rolls.FirstOrDefault(u => u.GuildId.Equals(guildId) && u.UserId.Equals(userId.AsLong())) ?? await CreateNewRow(guildId, userId);
            return user;
        }

        private async Task<RollsModel> CreateNewRow(ulong guildId, ulong userId)
        {
            var user = new RollsModel()
            {
                GuildId = guildId.AsLong(),
                UserId = userId.AsLong(),
                Rolls = 0
            };
            var newUser = _database.Rolls.Add(user).Entity;
            await _database.SaveChangesAsync();
            return newUser;
        }
    }
}