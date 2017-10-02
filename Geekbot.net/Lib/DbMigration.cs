using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Lib
{
    public class DbMigration
    {
        public static Task MigrateDatabaseToHash(IDatabase redis, ILogger logger)
        {
            foreach (var key in redis.Multiplexer.GetServer("127.0.0.1", 6379).Keys(6))
            {
                var keyParts = key.ToString().Split("-");
                if (keyParts.Length == 2 || keyParts.Length == 3)
                {
                    logger.Verbose($"Migrating key {key}");
                    var stuff = new List<string>();
                    stuff.Add("messages");
                    stuff.Add("karma");
                    stuff.Add("welcomeMsg");
                    stuff.Add("correctRolls");
                    if(stuff.Contains(keyParts[keyParts.Length - 1]))
                    {
                        var val = redis.StringGet(key);
                        ulong.TryParse(keyParts[0], out ulong guildId);
                        ulong.TryParse(keyParts[1], out ulong userId);

                        switch (keyParts[keyParts.Length - 1])
                        {
                            case "messages":
                                redis.HashSet($"{guildId}:Messages", new HashEntry[] { new HashEntry(userId.ToString(), val) });
                                break;
                            case "karma":
                                redis.HashSet($"{guildId}:Karma", new HashEntry[] { new HashEntry(userId.ToString(), val) });
                                break;
                            case "correctRolls":
                                redis.HashSet($"{guildId}:Rolls", new HashEntry[] { new HashEntry(userId.ToString(), val) });
                                break;
                            case "welcomeMsg":
                                redis.HashSet($"{guildId}:Settings", new HashEntry[] { new HashEntry("WelcomeMsg", val) });
                                break;
                        }
                    }
                    redis.KeyDelete(key);
                }
            }
            return Task.CompletedTask;
        }
    }
}