using System;
using StackExchange.Redis;

namespace Geekbot.net.Lib
{

//    public class RedisClient
//    {
//        private static readonly Lazy<RedisClient> _instance
//            = new Lazy<RedisClient>(() => new RedisClient());
//        private static readonly object ThreadLock = new object();
//        public static IDatabase Client;
//
//        private RedisClient()
//        { }
//
//        public static RedisClient Instance
//        {
//            get
//            {
//                lock (ThreadLock)
//                {
//                    if (Client == null)
//                    {
//                        try
//                        {
//                            var redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
//                            Client = redis.GetDatabase();
//                        }
//                        catch (Exception)
//                        {
//                            Console.WriteLine("Start Reids already you fucking faggot!");
//                            Environment.Exit(69);
//                        }
//                    }
//                }
//                return _instance.Value;
//            }
//        }
//    }

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
                Console.WriteLine("Start Redis already you fucking faggot!");
                Environment.Exit(69);
            }
        }

        public IDatabase Client { get; set; }
    }
}

