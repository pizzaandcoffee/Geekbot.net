using System;
using System.Linq;
using System.Threading.Tasks;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.AlmostRedis;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Logger;

namespace Geekbot.net.Database
{
    public class MessageMigration
    {
        private readonly DatabaseContext _database;
        private readonly IAlmostRedis _redis;
        private readonly IGeekbotLogger _logger;

        public MessageMigration(DatabaseContext database, IAlmostRedis redis, IGeekbotLogger logger)
        {
            _database = database;
            _redis = redis;
            _logger = logger;
        }

        public async Task Migrate()
        {
            _logger.Warning(LogSource.Migration, "Starting message migration");
            try
            {
                var messageKeys = _redis.GetAllKeys().Where(e => e.ToString().EndsWith("Messages"));
                foreach (var keyName in messageKeys)
                {
                    try
                    {
                        var guildId = ulong.Parse(keyName.ToString().Split(':').FirstOrDefault());
                        var guildUsers = _redis.Db.HashGetAll(keyName);
                        foreach (var user in guildUsers)
                        {
                            try
                            {
                                var userId = ulong.Parse(user.Name);
                                if (userId != 0)
                                {
                                    var userMessages = int.Parse(user.Value);
                                    _database.Messages.Add(new MessagesModel
                                    {
                                        UserId = userId.AsLong(),
                                        GuildId = guildId.AsLong(),
                                        MessageCount = userMessages
                                    });
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.Error(LogSource.Migration, $"Failed to add record for a user in {guildId}", e);
                            }
                        }

                        await _database.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        _logger.Error(LogSource.Migration, "Failed to determinate guild", e);
                    }
                }
                _logger.Warning(LogSource.Migration, "Successfully finished message migration");
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Migration, "Message migration failed", e);
            }
        }
    }
}