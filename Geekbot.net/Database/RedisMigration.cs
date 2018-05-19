using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Geekbot.net.Commands.Utils.Quote;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Logger;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Geekbot.net.Database
{
    public class RedisMigration
    {
        private readonly DatabaseContext _database;
        private readonly IDatabase _redis;
        private readonly IGeekbotLogger _logger;
        private readonly DiscordSocketClient _client;

        public RedisMigration(DatabaseContext database, IDatabase redis, IGeekbotLogger logger, DiscordSocketClient client)
        {
            _database = database;
            _redis = redis;
            _logger = logger;
            _client = client;
        }

        public async Task Migrate()
        {
            _logger.Information(LogSource.Geekbot, "Starting migration process");
            foreach (var guild in _client.Guilds)
            {
                _logger.Information(LogSource.Geekbot, $"Start Migration for {guild.Name}");
                #region Quotes
                /**
                 * Quotes
                 */
                try
                {
                    var data = _redis.SetScan($"{guild.Id}:Quotes");
                    foreach (var q in data)
                    {
                        try
                        {
                            var qd = JsonConvert.DeserializeObject<QuoteObjectDto>(q);
                            var quote = CreateQuoteObject(guild.Id, qd);
                            _database.Quotes.Add(quote);
                            await _database.SaveChangesAsync();
                        }
                        catch
                        {
                            _logger.Warning(LogSource.Geekbot, $"quote failed: {q}");
                        }
                    }
                }
                catch
                {
                    _logger.Warning(LogSource.Geekbot, "quote migration failed");
                }
                #endregion
                
                #region Karma
                /**
                 * Karma
                 */
                try
                {
                    var data = _redis.HashGetAll($"{guild.Id}:Karma");
                    foreach (var q in data)
                    {
                        try
                        {
                            var user = new KarmaModel()
                            {
                                GuildId = guild.Id.AsLong(),
                                UserId = ulong.Parse(q.Name).AsLong(),
                                Karma = int.Parse(q.Value),
                                TimeOut = DateTimeOffset.MinValue
                            };
                            _database.Karma.Add(user);
                            await _database.SaveChangesAsync();
                        }
                        catch
                        {
                            _logger.Warning(LogSource.Geekbot, $"karma failed for: {q.Name}");
                        }
                    }
                }
                catch
                {
                    _logger.Warning(LogSource.Geekbot, "karma migration failed");
                }
                #endregion
                
                #region Rolls
                /**
                 * Rolls
                 */
                try
                {
                    var data = _redis.HashGetAll($"{guild.Id}:Rolls");
                    foreach (var q in data)
                    {
                        try
                        {
                            var user = new RollsModel()
                            {
                                GuildId = guild.Id.AsLong(),
                                UserId = ulong.Parse(q.Name).AsLong(),
                                Rolls = int.Parse(q.Value)
                            };
                            _database.Rolls.Add(user);
                            await _database.SaveChangesAsync();
                        }
                        catch
                        {
                            _logger.Warning(LogSource.Geekbot, $"Rolls failed for: {q.Name}");
                        }
                    }
                }
                catch
                {
                    _logger.Warning(LogSource.Geekbot, "rolls migration failed");
                }
                #endregion
                
                #region Slaps
                /**
                 * Slaps
                 */
                try
                {
                    var given = _redis.HashGetAll($"{guild.Id}:SlapsGiven");
                    var gotten = _redis.HashGetAll($"{guild.Id}:SlapsGiven");
                    foreach (var q in given)
                    {
                        try
                        {
                            var user = new SlapsModel()
                            {
                                GuildId = guild.Id.AsLong(),
                                UserId = ulong.Parse(q.Name).AsLong(),
                                Given = int.Parse(q.Value),
                                Recieved= int.Parse(gotten[int.Parse(q.Name)].Value)
                            };
                            _database.Slaps.Add(user);
                            await _database.SaveChangesAsync();
                        }
                        catch
                        {
                            _logger.Warning(LogSource.Geekbot, $"Slaps failed for: {q.Name}");
                        }
                    }
                }
                catch
                {
                    _logger.Warning(LogSource.Geekbot, "Slaps migration failed");
                }
                #endregion
                
                #region Messages
                /**
                 * Messages
                 */
                try
                {
                    var data = _redis.HashGetAll($"{guild.Id}:Messages");
                    foreach (var q in data)
                    {
                        try
                        {
                            var user = new MessagesModel()
                            {
                                GuildId = guild.Id.AsLong(),
                                UserId = ulong.Parse(q.Name).AsLong(),
                                MessageCount= int.Parse(q.Value)
                            };
                            _database.Messages.Add(user);
                            await _database.SaveChangesAsync();
                        }
                        catch
                        {
                            _logger.Warning(LogSource.Geekbot, $"Messages failed for: {q.Name}");
                        }
                    }
                }
                catch
                {
                    _logger.Warning(LogSource.Geekbot, "Messages migration failed");
                }
                #endregion
                
                #region Ships
                /**
                 * Ships
                 */
                try
                {
                    var data = _redis.HashGetAll($"{guild.Id}:Ships");
                    var done = new List<string>();
                    foreach (var q in data)
                    {
                        try
                        {
                            if (done.Contains(q.Name)) continue;
                            var split = q.Name.ToString().Split('-');
                            var user = new ShipsModel()
                            {
                                FirstUserId = ulong.Parse(split[0]).AsLong(),
                                SecondUserId = ulong.Parse(split[1]).AsLong(),
                                Strength = int.Parse(q.Value)
                            };
                            _database.Ships.Add(user);
                            await _database.SaveChangesAsync();
                            done.Add(q.Name);
                        }
                        catch
                        {
                            _logger.Warning(LogSource.Geekbot, $"Ships failed for: {q.Name}");
                        }
                    }
                }
                catch
                {
                    _logger.Warning(LogSource.Geekbot, "Ships migration failed");
                }
                #endregion
                
                #region GuildSettings
                /**
                 * Users
                 */
                try
                {
                    var data = _redis.HashGetAll($"{guild.Id}:Settings");
                    var settings = new GuildSettingsModel()
                    {
                        GuildId = guild.Id.AsLong(),
                        Hui = true
                    };
                    foreach (var setting in data)
                    {
                        try
                        {
                            switch (setting.Name)
                            {
                                case "ShowLeave":
                                    settings.ShowLeave = setting.Value.ToString() == "1";
                                    break;
                                case "ShowDelete":
                                    settings.ShowDelete = setting.Value.ToString() == "1";
                                    break;
                                case "WikiLang":
                                    settings.WikiLang = setting.Value.ToString();
                                    break;
                                case "Language":
                                    settings.Language = setting.Value.ToString();
                                    break;
                                case "WelcomeMsg":
                                    settings.WelcomeMessage = setting.Value.ToString();
                                    break;
                                case "ping":
                                    settings.Ping = bool.Parse(setting.Value.ToString());
                                    break;
                                case "ModChannel":
                                    settings.ModChannel = long.Parse(setting.Value);
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.Warning(LogSource.Geekbot, $"Setting failed: {setting.Name} - {guild.Id}", e);
                        }
                    }
                }
                catch
                {
                    _logger.Warning(LogSource.Geekbot, "Settings migration failed");
                }

                #endregion
                
                #region Users
                /**
                 * Users
                 */
                try
                {
                    var data = guild.Users;
                    foreach (var user in data)
                    {
                        try
                        {
                            var namesSerialized = _redis.HashGet($"User:{user.Id}", "UsedNames").ToString();
                            var names = namesSerialized != null
                                ? Utf8Json.JsonSerializer.Deserialize<string[]>(namesSerialized)
                                : new string[] {user.Username};
                            _database.Users.AddIfNotExists(new UserModel()
                            {
                                UserId = user.Id.AsLong(),
                                Username = user.Username,
                                Discriminator = user.Discriminator,
                                AvatarUrl = user.GetAvatarUrl(ImageFormat.Auto, 1024),
                                IsBot = user.IsBot,
                                Joined = user.CreatedAt,
                                UsedNames = names.Select(name => new UserUsedNamesModel() {Name = name, FirstSeen = DateTimeOffset.Now}).ToList()
                            }, model => model.UserId.Equals(user.Id.AsLong()));
                            await _database.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            _logger.Warning(LogSource.Geekbot, $"User failed: {user.Username}", e);
                        }
                    }
                }
                catch
                {
                    _logger.Warning(LogSource.Geekbot, "User migration failed");
                }
                #endregion
                
                #region Guilds

                _database.Guilds.Add(new GuildsModel
                {
                    CreatedAt = guild.CreatedAt,
                    GuildId = guild.Id.AsLong(),
                    IconUrl = guild.IconUrl,
                    Name = guild.Name,
                    Owner = guild.Owner.Id.AsLong()
                });
                await _database.SaveChangesAsync();

                #endregion
                _logger.Information(LogSource.Geekbot, $"Finished Migration for {guild.Name}");
            }
            _logger.Information(LogSource.Geekbot, "Finished migration process");
        }
        
        private QuoteModel CreateQuoteObject(ulong guild, QuoteObjectDto quote)
        {
            var last = _database.Quotes.Where(e => e.GuildId.Equals(guild.AsLong()))
                .OrderByDescending(e => e.InternalId).FirstOrDefault();
            int internalId = 1;
            if (last != null) internalId = last.InternalId + 1;
            return new QuoteModel()
            {
                InternalId = internalId,
                GuildId = guild.AsLong(),
                UserId = quote.UserId.AsLong(),
                Time = quote.Time,
                Quote = quote.Quote,
                Image = quote.Image
            };
        }
    }
}