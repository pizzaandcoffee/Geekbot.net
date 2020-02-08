using System.Collections.Generic;
using System.Linq;
using Geekbot.net.Database;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.UserRepository;

namespace Geekbot.net.Lib.Highscores
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

        public Dictionary<HighscoreUserDto, int> GetHighscoresWithUserData(HighscoreTypes type, ulong guildId, int amount)
        {
            Dictionary<ulong, int> list;
            switch (type)
            {
                case HighscoreTypes.messages:
                    list = GetMessageList(guildId, amount);
                    break;
                case HighscoreTypes.karma:
                    list = GetKarmaList(guildId, amount);
                    break;
                case HighscoreTypes.rolls:
                    list = GetRollsList(guildId, amount);
                    break;
                case HighscoreTypes.cookies:
                    list = GetCookiesList(guildId, amount);
                    break;
                default:
                    list = new Dictionary<ulong, int>();
                    break;
            }
            
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
        
        public Dictionary<ulong, int> GetCookiesList(ulong guildId, int amount)
        {
            return _database.Cookies
                .Where(k => k.GuildId.Equals(guildId.AsLong()))
                .OrderByDescending(o => o.Cookies)
                .Take(amount)
                .ToDictionary(key => key.UserId.AsUlong(), key => key.Cookies);
        }
    }
}