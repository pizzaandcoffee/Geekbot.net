using System.Threading.Tasks;
using Discord.Commands;
using StackExchange.Redis;

namespace Geekbot.net.Modules
{
    [Group("admin")]
    public class AdminCmd : ModuleBase
    {
        private readonly IDatabase redis;
        public AdminCmd(IDatabase redis)
        {
            this.redis = redis;
        }

        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [Command("welcome", RunMode = RunMode.Async), Summary("Set a Welcome Message (use '$user' to mention the new joined user).")]
        public async Task SetWelcomeMessage([Remainder, Summary("The message")] string welcomeMessage)
        {
            var key = Context.Guild.Id + "-welcomeMsg";
            redis.StringSet(key, welcomeMessage);
            var formatedMessage = welcomeMessage.Replace("$user", Context.User.Mention);
            await ReplyAsync("Welcome message has been changed\r\nHere is an example of how it would look:\r\n" +
                        formatedMessage);
        }

        [Command("youtubekey", RunMode = RunMode.Async), Summary("Set the youtube api key")]
        public async Task SetYoutubeKey([Summary("API Key")] string key)
        {
            var botOwner = redis.StringGet("botOwner");
            if (!Context.User.Id.ToString().Equals(botOwner.ToString()))
            {
                await ReplyAsync($"Sorry, only the botowner can do this ({botOwner}");
                return;
            }

            redis.StringSet("youtubeKey", key);
            await ReplyAsync("Apikey has been set");
        }
    }
}