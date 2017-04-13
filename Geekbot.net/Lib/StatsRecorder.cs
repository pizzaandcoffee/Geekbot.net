using System;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Geekbot.net.Lib
{
    public class StatsRecorder
    {

        private SocketMessage message;

        public StatsRecorder(SocketMessage message)
        {
            this.message = message;
        }

        public async Task UpdateUserRecordAsync()
        {
            Console.WriteLine(message.Author.Username + " earned a point");
            await Task.FromResult(true);
        }

        public async Task UpdateGuildRecordAsync()
        {
            var channel = (SocketGuildChannel) message.Channel;
            Console.WriteLine(channel.Guild.Name + " earned a point");
            await Task.FromResult(true);
        }
    }
}