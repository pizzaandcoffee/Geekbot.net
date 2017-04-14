using System;
using StackExchange.Redis;

namespace Geekbot.net.Lib
{
    public interface IRedisClient
    {
        IDatabase Client { get; set; }
    }

    public sealed class RedisClient : IRedisClient
    {
        public RedisClient()
        {
            var redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
            if (!redis.IsConnected)
            {
                Console.WriteLine("Could not Connect to the Server...");
            }
            Client = redis.GetDatabase();
        }

        public IDatabase Client { get; set; }
    }
}

