using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;

namespace Geekbot.net.Commands.Utils
{
    public class Info : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IErrorHandler _errorHandler;

        public Info(IErrorHandler errorHandler, DiscordSocketClient client, CommandService commands)
        {
            _errorHandler = errorHandler;
            _client = client;
            _commands = commands;
        }

        [Command("info", RunMode = RunMode.Async)]
        [Summary("Get Information about the bot")]
        public async Task BotInfo()
        {
            try
            {
                var eb = new EmbedBuilder();

                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(_client.CurrentUser.GetAvatarUrl())
                    .WithName($"{Constants.Name} V{Constants.BotVersion()}"));
                var botOwner = (await _client.GetApplicationInfoAsync()).Owner;
                var uptime = DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime);

                eb.AddInlineField("Bot Name", _client.CurrentUser.Username);
                eb.AddInlineField("Bot Owner", $"{botOwner.Username}#{botOwner.Discriminator}");
                eb.AddInlineField("Library", "Discord.NET V1.0.2");
                eb.AddInlineField("Uptime", $"{uptime.Days}D {uptime.Hours}H {uptime.Minutes}M {uptime.Seconds}S");
                eb.AddInlineField("Servers", Context.Client.GetGuildsAsync().Result.Count);
                eb.AddInlineField("Total Commands", _commands.Commands.Count());

                eb.AddField("Website", "https://geekbot.pizzaandcoffee.rocks/");

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("uptime", RunMode = RunMode.Async)]
        [Summary("Get the Bot Uptime")]
        public async Task BotUptime()
        {
            try
            {
                var uptime = DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime);
                await ReplyAsync($"{uptime.Days}D {uptime.Hours}H {uptime.Minutes}M {uptime.Seconds}S");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}