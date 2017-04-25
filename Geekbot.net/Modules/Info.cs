using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
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

        [Command("info", RunMode = RunMode.Async), Summary("Get Information about the bot")]
        public async Task BotInfo()
        {
            var eb = new EmbedBuilder();

            eb.WithTitle("Geekbot V3");

            var botOwner = Context.Guild.GetUserAsync(ulong.Parse(redis.Client.StringGet("botOwner"))).Result;

            eb.AddInlineField("Status", Context.Client.ConnectionState.ToString())
                .AddInlineField("Bot Name", Context.Client.CurrentUser.Username)
                .AddInlineField("Bot Owner", $"{botOwner.Username}#{botOwner.Discriminator}");

            eb.AddInlineField("Servers", Context.Client.GetGuildsAsync().Result.Count);

            await ReplyAsync("", false, eb.Build());
        }
    }
}