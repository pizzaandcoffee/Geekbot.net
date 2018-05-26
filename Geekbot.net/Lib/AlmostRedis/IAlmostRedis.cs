using System.Collections.Generic;
using StackExchange.Redis;

namespace Geekbot.net.Lib.AlmostRedis
{
    public interface IAlmostRedis
    {
        void Connect();
        IDatabase Db { get; }
        ConnectionMultiplexer Connection { get; }
        IEnumerable<RedisKey> GetAllKeys();
    }
}