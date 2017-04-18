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
            try
            {
                var redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
                Client = redis.GetDatabase();
            }
            catch (Exception)
            {
                Console.WriteLine("Start Redis pls...");
                Environment.Exit(1);
            }
        }

        public IDatabase Client { get; set; }
    }
}

