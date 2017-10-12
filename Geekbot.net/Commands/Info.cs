using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class Info : ModuleBase
    {
        private readonly IDatabase _redis;
        private readonly IErrorHandler _errorHandler;
        private readonly DiscordSocketClient _client;

        public Info(IDatabase redis, IErrorHandler errorHandler, DiscordSocketClient client)
        {
            _redis = redis;
            _errorHandler = errorHandler;
            _client = client;
        }

        [Command("info", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Get Information about the bot")]
        public async Task BotInfo()
        {
            try
            {
                var eb = new EmbedBuilder();
                
                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(_client.CurrentUser.GetAvatarUrl())
                    .WithName($"{Constants.Name} V{Constants.BotVersion}"));
                var botOwner = await Context.Guild.GetUserAsync(ulong.Parse(_redis.StringGet("botOwner")));
                var uptime = (DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime));
                
                eb.AddInlineField("Bot Name", _client.CurrentUser.Username);
                eb.AddInlineField("Servers", Context.Client.GetGuildsAsync().Result.Count);
                eb.AddInlineField("Uptime", $"{uptime.Days}D {uptime.Hours}H {uptime.Minutes}M {uptime.Seconds}S");
                eb.AddInlineField("Bot Owner", $"{botOwner.Username}#{botOwner.Discriminator}");
                eb.AddInlineField("Website", "https://geekbot.pizzaandcoffee.rocks/");

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);              
            }
        }
        
        [Command("uptime", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Get the Bot Uptime")]
        public async Task BotUptime()
        {
            try
            {
                var uptime = (DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime));
                await ReplyAsync($"{uptime.Days}D {uptime.Hours}H {uptime.Minutes}M {uptime.Seconds}S");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);              
            }
        }
    }
}