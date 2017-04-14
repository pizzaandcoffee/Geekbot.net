using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    [Group("admin")]
    public class AdminCmd : ModuleBase
    {
        [Command("welcome"), Summary("Set a Welcome Message (use '$user' to mention the new joined user).")]
        public async Task SetWelcomeMessage([Remainder, Summary("The message")] string welcomeMessage)
        {
            if (Context.Guild.OwnerId.Equals(Context.User.Id))
            {
                var redis = new RedisClient().Client;
                var key = Context.Guild.Id + "-welcome-msg";
                redis.StringSet(key, welcomeMessage);
                var formatedMessage = welcomeMessage.Replace("$user", Context.User.Mention);
                await ReplyAsync("W!elcome message has been changed\r\nHere is an example of how it would look:\r\n" +
                           formatedMessage);
            }
            else
            {
                await ReplyAsync("Sorry, only the Server Owner can do this");
            }

        }
    }
}