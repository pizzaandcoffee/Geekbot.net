using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    public class Info : ModuleBase
    {
        private readonly IRedisClient redis;
        public Info(IRedisClient redisClient)
        {
            redis = redisClient;
        }

        [Command("info"), Summary("Show some info about the bot.")]
        public async Task getInfo()
        {
            var eb = new EmbedBuilder();
            eb.WithTitle("Geekbot Information");
            eb.WithColor(new Color(110, 204, 147));

            eb.AddInlineField("Version", "3.1")
                .AddInlineField("Uptime", "Not Calculated...");
            await ReplyAsync("", false, eb.Build());
        }
    }
}