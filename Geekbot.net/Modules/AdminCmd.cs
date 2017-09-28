using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Modules
{
    [Group("admin")]
    public class AdminCmd : ModuleBase
    {
        private readonly IDatabase redis;
        private readonly DiscordSocketClient client;
        private readonly ILogger logger;
        
        public AdminCmd(IDatabase redis, DiscordSocketClient client, ILogger logger)
        {
            this.redis = redis;
            this.client = client;
            this.logger = logger;
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("welcome", RunMode = RunMode.Async)]
        [Summary("Set a Welcome Message (use '$user' to mention the new joined user).")]
        public async Task SetWelcomeMessage([Remainder] [Summary("message")] string welcomeMessage)
        {
            redis.HashSet($"{Context.Guild.Id}:Settings", new HashEntry[] { new HashEntry("WelcomeMsg", welcomeMessage) });
            var formatedMessage = welcomeMessage.Replace("$user", Context.User.Mention);
            await ReplyAsync("Welcome message has been changed\r\nHere is an example of how it would look:\r\n" +
                             formatedMessage);
        }

        [Command("youtubekey", RunMode = RunMode.Async)]
        [Summary("Set the youtube api key")]
        public async Task SetYoutubeKey([Summary("API Key")] string key)
        {
            var botOwner = Context.Guild.GetUserAsync(ulong.Parse(redis.StringGet("botOwner"))).Result;
            if (!Context.User.Id.ToString().Equals(botOwner.Id.ToString()))
            {
                await ReplyAsync($"Sorry, only the botowner can do this ({botOwner.Username}#{botOwner.Discriminator})");
                return;
            }

            redis.StringSet("youtubeKey", key);
            await ReplyAsync("Apikey has been set");
        }
        
        [Command("game", RunMode = RunMode.Async)]
        [Summary("Set the game that the bot is playing")]
        public async Task SetGame([Remainder] [Summary("Game")] string key)
        {
            var botOwner = Context.Guild.GetUserAsync(ulong.Parse(redis.StringGet("botOwner"))).Result;
            if (!Context.User.Id.ToString().Equals(botOwner.Id.ToString()))
            {
                await ReplyAsync($"Sorry, only the botowner can do this ({botOwner.Username}#{botOwner.Discriminator})");
                return;
            }

            redis.StringSet("Game", key);
            await client.SetGameAsync(key);
            logger.Information($"[Geekbot] Changed game to {key}");
            await ReplyAsync($"Now Playing {key}");
        }
    }
}