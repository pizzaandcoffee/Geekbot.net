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
        private IDatabase _database;
        private ConnectionMultiplexer _connection;
        private IAlmostRedis _almostRedisImplementation;

        public AlmostRedis(GeekbotLogger logger, RunParameters runParameters)
        {
            _logger = logger;
            _runParameters = runParameters;
        }

        public void Connect()
        {
            _connection = ConnectionMultiplexer.Connect($"{_runParameters.RedisHost}:{_runParameters.RedisPort}");
            _database = _connection.GetDatabase(int.Parse(_runParameters.RedisDatabase));
            _logger.Information(LogSource.Redis, $"Connected to Redis on {_connection.Configuration} at {_database.Database}");
        }

        public IDatabase Db
        {
            get { return _database; }
        }

        public ConnectionMultiplexer Connection
        {
            get { return _connection; }
        }
        
        public IEnumerable<RedisKey> GetAllKeys()
        {
            return _connection.GetServer($"{_runParameters.RedisHost}:{_runParameters.RedisPort}", int.Parse(_runParameters.RedisDatabase)).Keys();
        }
    }
}