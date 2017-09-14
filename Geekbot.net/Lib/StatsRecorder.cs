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

        public StatsRecorder(SocketMessage message, IDatabase redis)
        {
            this.message = message;
            this.redis = redis;
        }

        public async Task UpdateUserRecordAsync()
        {
            var guildId = ((SocketGuildChannel) message.Channel).Guild.Id;
            var key = guildId + "-" + message.Author.Id + "-messages";
            await redis.StringIncrementAsync(key);
        }

        public async Task UpdateGuildRecordAsync()
        {
            var guildId = ((SocketGuildChannel) message.Channel).Guild.Id;
            var key = guildId + "-messages";
            await redis.StringIncrementAsync(key);
        }
    }
}