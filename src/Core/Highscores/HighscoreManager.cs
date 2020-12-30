using System.Collections.Generic;
using System.Linq;
using Geekbot.Core.Database;
using Geekbot.Core.Extensions;
using Geekbot.Core.UserRepository;

namespace Geekbot.Core.Highscores
{
    public class HighscoreManager : IHighscoreManager
    {
        private readonly DatabaseContext _database;
        private readonly IUserRepository _userRepository;

        public HighscoreManager(DatabaseContext databaseContext, IUserRepository userRepository)
        {
            _database = databaseContext;
            _userRepository = userRepository;
            
        }

        public Dictionary<HighscoreUserDto, int> GetHighscoresWithUserData(HighscoreTypes type, ulong guildId, int amount, string season = null)
        {
            var list = type switch
            {
                HighscoreTypes.messages => GetMessageList(guildId, amount),
                HighscoreTypes.karma => GetKarmaList(guildId, amount),
                HighscoreTypes.rolls => GetRollsList(guildId, amount),
                HighscoreTypes.cookies => GetCookiesList(guildId, amount),
                HighscoreTypes.seasons => GetMessageSeasonList(guildId, amount, season),
                HighscoreTypes.quotes => GetQuotesList(guildId, amount),
                _ => new Dictionary<ulong, int>()
            };

            if (!list.Any())
            {
                throw new HighscoreListEmptyException($"No {type} found for guild {guildId}");
            }
            
            var highscoreUsers = new Dictionary<HighscoreUserDto, int>();
            foreach (var user in list)
            {
                try
                {
                    var guildUser = _userRepository.Get(user.Key);
                    if (guildUser?.Username != null)
                    {
                        highscoreUsers.Add(new HighscoreUserDto
                        {
                            Username = guildUser.Username,
                            Discriminator = guildUser.Discriminator,
                            Avatar = guildUser.AvatarUrl
                        },  user.Value);
                    }
                    else
                    {
                        highscoreUsers.Add(new HighscoreUserDto
                        {
                            Id = user.Key.ToString()
                        },  user.Value);
                    }
                }
                catch
                {
                    // ignore
                }
            }

            return highscoreUsers;
        }
        
        public Dictionary<ulong, int> GetMessageList(ulong guildId, int amount)
        {
            return _database.Messages
                .Where(k => k.GuildId.Equals(guildId.AsLong()))
                .OrderByDescending(o => o.MessageCount)
                .Take(amount)
                .ToDictionary(key => key.UserId.AsUlong(), key => key.MessageCount);
        }
        
        public Dictionary<ulong, int> GetMessageSeasonList(ulong guildId, int amount, string season)
        {
            if (string.IsNullOrEmpty(season))
            {
                season = SeasonsUtils.GetCurrentSeason();
            }
            return _database.MessagesSeasons
                .Where(k => k.GuildId.Equals(guildId.AsLong()) && k.Season.Equals(season))
                .OrderByDescending(o => o.MessageCount)
                .Take(amount)
                .ToDictionary(key => key.UserId.AsUlong(), key => key.MessageCount);
        }
        
        public Dictionary<ulong, int> GetKarmaList(ulong guildId, int amount)
        {
            return _database.Karma
                .Where(k => k.GuildId.Equals(guildId.AsLong()))
                .OrderByDescending(o => o.Karma)
                .Take(amount)
                .ToDictionary(key => key.UserId.AsUlong(), key => key.Karma);
        }
        
        public Dictionary<ulong, int> GetRollsList(ulong guildId, int amount)
        {
            return _database.Rolls
                .Where(k => k.GuildId.Equals(guildId.AsLong()))
                .OrderByDescending(o => o.Rolls)
                .Take(amount)
                .ToDictionary(key => key.UserId.AsUlong(), key => key.Rolls);
        }

        private Dictionary<ulong, int> GetCookiesList(ulong guildId, int amount)
        {
            return _database.Cookies
                .Where(k => k.GuildId.Equals(guildId.AsLong()))
                .OrderByDescending(o => o.Cookies)
                .Take(amount)
                .ToDictionary(key => key.UserId.AsUlong(), key => key.Cookies);
        }
        
        private Dictionary<ulong, int> GetQuotesList(ulong guildId, int amount)
        {
            return _database.Quotes
                .Where(row => row.GuildId == guildId.AsLong())
                .GroupBy(row => row.UserId)
                .Select(row => new { userId = row.Key, amount = row.Count()})
                .OrderByDescending(row => row.amount)
                .Take(amount)
                .ToDictionary(key => key.userId.AsUlong(), key => key.amount);
        }
    }
}