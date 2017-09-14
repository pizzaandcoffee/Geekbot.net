using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using StackExchange.Redis;

namespace Geekbot.net.Modules
{
    public class Info : ModuleBase
    {
        private readonly IDatabase redis;
        public Info(IDatabase redis)
        {
            this.redis = redis;
        }

        [Command("info", RunMode = RunMode.Async), Summary("Get Information about the bot")]
        public async Task BotInfo()
        {
            var eb = new EmbedBuilder();

            eb.WithTitle("Geekbot V3.1");

            var botOwner = Context.Guild.GetUserAsync(ulong.Parse(redis.StringGet("botOwner"))).Result;

            eb.AddInlineField("Status", Context.Client.ConnectionState.ToString())
                .AddInlineField("Bot Name", Context.Client.CurrentUser.Username)
                .AddInlineField("Bot Owner", $"{botOwner.Username}#{botOwner.Discriminator}");

            eb.AddInlineField("Servers", Context.Client.GetGuildsAsync().Result.Count);

            await ReplyAsync("", false, eb.Build());
        }
    }
}