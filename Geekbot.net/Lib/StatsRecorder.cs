using System;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Geekbot.net.Lib
{
    public class StatsRecorder
    {

        public static async Task Record(SocketMessage message)
        {
            await UpdateUserRecordTask(message);
        }

        private static Task UpdateUserRecordTask(SocketMessage message)
        {
            return Task.Run(() => UpdateUserRecord(message));

        }

        private static void UpdateUserRecord(SocketMessage message)
        {
            Console.WriteLine(message.Author.Username + " earned a point");
        }
    }
}