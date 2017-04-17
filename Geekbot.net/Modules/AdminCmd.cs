using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    [Group("admin")]
    public class AdminCmd : ModuleBase
    {
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [Command("welcome"), Summary("Set a Welcome Message (use '$user' to mention the new joined user).")]
        public async Task SetWelcomeMessage([Remainder, Summary("The message")] string welcomeMessage)
        {
            var redis = new RedisClient().Client;
            var key = Context.Guild.Id + "-welcomeMsg";
            redis.StringSet(key, welcomeMessage);
            var formatedMessage = welcomeMessage.Replace("$user", Context.User.Mention);
            await ReplyAsync("Welcome message has been changed\r\nHere is an example of how it would look:\r\n" +
                        formatedMessage);
        }
    }
}