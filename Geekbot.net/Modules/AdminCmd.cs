using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    [Group("admin")]
    public class AdminCmd : ModuleBase
    {
        private readonly IRedisClient redis;
        public AdminCmd(IRedisClient redisClient)
        {
            redis = redisClient;
        }

        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [Command("welcome", RunMode = RunMode.Async), Summary("Set a Welcome Message (use '$user' to mention the new joined user).")]
        public async Task SetWelcomeMessage([Remainder, Summary("The message")] string welcomeMessage)
        {
            var key = Context.Guild.Id + "-welcomeMsg";
            redis.Client.StringSet(key, welcomeMessage);
            var formatedMessage = welcomeMessage.Replace("$user", Context.User.Mention);
            await ReplyAsync("Welcome message has been changed\r\nHere is an example of how it would look:\r\n" +
                        formatedMessage);
        }
    }
}