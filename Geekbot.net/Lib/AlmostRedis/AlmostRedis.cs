using System.Collections.Generic;
using Geekbot.net.Lib.Logger;
using StackExchange.Redis;

namespace Geekbot.net.Lib.AlmostRedis
{
    // if anyone ever sees this, please come up with a better fucking name, i'd appriciate it
    public class AlmostRedis : IAlmostRedis
    {
        private readonly GeekbotLogger _logger;
        private readonly RunParameters _runParameters;

        public AlmostRedis(GeekbotLogger logger, RunParameters runParameters)
        {
            _logger = logger;
            _runParameters = runParameters;
        }

        public void Connect()
        {
            Connection = ConnectionMultiplexer.Connect($"{_runParameters.RedisHost}:{_runParameters.RedisPort}");
            Db = Connection.GetDatabase(int.Parse(_runParameters.RedisDatabase));
            _logger.Information(LogSource.Redis, $"Connected to Redis on {Connection.Configuration} at {Db.Database}");
        }

        public IDatabase Db { get; private set; }

        public ConnectionMultiplexer Connection { get; private set; }

        public IEnumerable<RedisKey> GetAllKeys()
        {
            return Connection.GetServer($"{_runParameters.RedisHost}:{_runParameters.RedisPort}").Keys(int.Parse(_runParameters.RedisDatabase));
        }
    }
}