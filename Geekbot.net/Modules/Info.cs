using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Modules
{
    public class Info : ModuleBase
    {
        private readonly IDatabase _redis;
        private readonly IErrorHandler _errorHandler;

        public Info(IDatabase redis, IErrorHandler errorHandler)
        {
            _redis = redis;
            _errorHandler = errorHandler;
        }

        [Command("info", RunMode = RunMode.Async)]
        [Summary("Get Information about the bot")]
        public async Task BotInfo()
        {
            try
            {
                var eb = new EmbedBuilder();

                eb.WithTitle("Geekbot V3.3");

                var botOwner = Context.Guild.GetUserAsync(ulong.Parse(_redis.StringGet("botOwner"))).Result;
                var uptime = (DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime));
                
                eb.AddInlineField("Bot Name", Context.Client.CurrentUser.Username)
                    .AddInlineField("Servers", Context.Client.GetGuildsAsync().Result.Count)
                    .AddInlineField("Uptime", $"{uptime.Days}D {uptime.Hours}H {uptime.Minutes}M {uptime.Seconds}S")
                    .AddInlineField("Bot Owner", $"{botOwner.Username}#{botOwner.Discriminator}")
                    .AddInlineField("Website", "https://geekbot.pizzaandcoffee.rocks/");

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);              
            }
        }
    }
}