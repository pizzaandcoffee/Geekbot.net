using System;
using StackExchange.Redis;

namespace Geekbot.net.Lib
{
    public sealed class RedisSingleton
    {
        private RedisSingleton()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            if (redis.IsConnected)
            {
                Console.WriteLine("Connection to Redis Enstablished");
            }
            else
            {
                Console.WriteLine("Connection to Redis Failed");
            }
        }
        private static readonly Lazy<RedisSingleton> lazy = new Lazy<RedisSingleton>(() => new RedisSingleton());
        public static RedisSingleton Instance
        {
            get
            {
                return lazy.Value;
            }
        }
    }
}

