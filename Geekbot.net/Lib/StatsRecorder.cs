using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using StackExchange.Redis;

namespace Geekbot.net.Lib
{
    public class StatsRecorder
    {

        private readonly SocketMessage message;
        private readonly IDatabase redis;

        public StatsRecorder(SocketMessage message, IRedisClient redisClient)
        {
            this.message = message;
            redis = redisClient.Client;
        }

        public async Task UpdateUserRecordAsync()
        {
            var guildId = ((SocketGuildChannel) message.Channel).Guild.Id;
            var key = guildId + "-" + message.Author.Id + "-messages";
            var messages = (int)redis.StringGet(key);
            redis.StringSet(key, (messages + 1).ToString());
        }

        public async Task UpdateGuildRecordAsync()
        {
            var guildId = ((SocketGuildChannel) message.Channel).Guild.Id;
            var key = guildId + "-messages";
            var messages = (int)redis.StringGet(key);
            redis.StringSet(key, (messages + 1).ToString());
        }
    }
}