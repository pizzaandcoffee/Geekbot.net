using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib.CommandPreconditions;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.GuildSettingsManager;
using Geekbot.net.Lib.Localization;

namespace Geekbot.net.Commands.Admin
{
    [Group("admin")]
    [RequireUserPermission(GuildPermission.Administrator)]
    [DisableInDirectMessage]
    public class Admin : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly IErrorHandler _errorHandler;
        private readonly IGuildSettingsManager _guildSettingsManager;
        private readonly ITranslationHandler _translation;

        public Admin(DiscordSocketClient client, IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager, ITranslationHandler translationHandler)
        {
            _client = client;
            _errorHandler = errorHandler;
            _guildSettingsManager = guildSettingsManager;
            _translation = translationHandler;
        }

        [Command("welcome", RunMode = RunMode.Async)]
        [Summary("Set a Welcome Message (use '$user' to mention the new joined user).")]
        public async Task SetWelcomeMessage([Remainder, Summary("message")] string welcomeMessage)
        {
            var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
            guild.WelcomeMessage = welcomeMessage;
            await _guildSettingsManager.UpdateSettings(guild);

            var formatedMessage = welcomeMessage.Replace("$user", Context.User.Mention);
            await ReplyAsync($"Welcome message has been changed\r\nHere is an example of how it would look:\r\n{formatedMessage}");
        }

        [Command("welcomechannel", RunMode = RunMode.Async)]
        [Summary("Set a channel for the welcome messages (by default it uses the top most channel)")]
        public async Task SelectWelcomeChannel([Summary("#Channel")] ISocketMessageChannel channel)
        {
            try
            {
                var m = await channel.SendMessageAsync("...");

                var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                guild.WelcomeChannel = channel.Id.AsLong();
                await _guildSettingsManager.UpdateSettings(guild);

                await m.DeleteAsync();

                await ReplyAsync("Successfully saved the welcome channel");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context, "That channel doesn't seem to exist or i don't have write permissions");
            }
        }

        [Command("modchannel", RunMode = RunMode.Async)]
        [Summary("Set a channel for moderation purposes")]
        public async Task SelectModChannel([Summary("#Channel")] ISocketMessageChannel channel)
        {
            try
            {
                var m = await channel.SendMessageAsync("verifying...");

                var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                guild.ModChannel = channel.Id.AsLong();
                await _guildSettingsManager.UpdateSettings(guild);

                var sb = new StringBuilder();
                sb.AppendLine("Successfully saved mod channel, you can now do the following");
                sb.AppendLine("- `!admin showleave` - send message to mod channel when someone leaves");
                sb.AppendLine("- `!admin showdel` - send message to mod channel when someone deletes a message");
                await m.ModifyAsync(e => e.Content = sb.ToString());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context, "That channel doesn't seem to exist or i don't have write permissions");
            }
        }

        [Command("showleave", RunMode = RunMode.Async)]
        [Summary("Toggle - notify modchannel when someone leaves")]
        public async Task ShowLeave()
        {
            try
            {
                var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                var modChannel = await GetModChannel(guild.ModChannel.AsUlong());
                if (modChannel == null) return;

                guild.ShowLeave = !guild.ShowLeave;
                await _guildSettingsManager.UpdateSettings(guild);
                await modChannel.SendMessageAsync(guild.ShowLeave
                    ? "Saved - now sending messages here when someone leaves"
                    : "Saved - stopping sending messages here when someone leaves"
                );
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("showdel", RunMode = RunMode.Async)]
        [Summary("Toggle - notify modchannel when someone deletes a message")]
        public async Task ShowDelete()
        {
            try
            {
                var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                var modChannel = await GetModChannel(guild.ModChannel.AsUlong());
                if (modChannel == null) return;

                guild.ShowDelete = !guild.ShowDelete;
                await _guildSettingsManager.UpdateSettings(guild);
                await modChannel.SendMessageAsync(guild.ShowDelete
                    ? "Saved - now sending messages here when someone deletes a message"
                    : "Saved - stopping sending messages here when someone deletes a message"
                );
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("setlang", RunMode = RunMode.Async)]
        [Summary("Change the bots language")]
        public async Task SetLanguage([Summary("language")] string languageRaw)
        {
            try
            {
                var language = languageRaw.ToUpper();
                var success = await _translation.SetLanguage(Context.Guild.Id, language);
                if (success)
                {
                    var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                    guild.Language = language;
                    await _guildSettingsManager.UpdateSettings(guild);

                    var transContext = await _translation.GetGuildContext(Context);
                    await ReplyAsync(transContext.GetString("NewLanguageSet"));
                    return;
                }

                await ReplyAsync(
                    $"That doesn't seem to be a supported language\r\nSupported Languages are {string.Join(", ", _translation.SupportedLanguages)}");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("wiki", RunMode = RunMode.Async)]
        [Summary("Change the wikipedia instance (use lang code in xx.wikipedia.org)")]
        public async Task SetWikiLanguage([Summary("language")] string languageRaw)
        {
            try
            {
                var language = languageRaw.ToLower();
                var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                guild.WikiLang = language;
                await _guildSettingsManager.UpdateSettings(guild);

                await ReplyAsync($"Now using the {language} wikipedia");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("ping", RunMode = RunMode.Async)]
        [Summary("Enable the ping reply.")]
        public async Task TogglePing()
        {
            try
            {
                var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                guild.Ping = !guild.Ping;
                await _guildSettingsManager.UpdateSettings(guild);
                await ReplyAsync(guild.Ping ? "i will reply to ping now" : "No more pongs...");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("hui", RunMode = RunMode.Async)]
        [Summary("Enable the ping reply.")]
        public async Task ToggleHui()
        {
            try
            {
                var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                guild.Hui = !guild.Hui;
                await _guildSettingsManager.UpdateSettings(guild);
                await ReplyAsync(guild.Hui ? "i will reply to hui now" : "No more hui's...");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task<ISocketMessageChannel> GetModChannel(ulong channelId)
        {
            try
            {
                if (channelId == ulong.MinValue) throw new Exception();
                var modChannel = (ISocketMessageChannel) _client.GetChannel(channelId);
                if (modChannel == null) throw new Exception();
                return modChannel;
            }
            catch
            {
                await ReplyAsync("Modchannel doesn't seem to exist, please set one with `!admin modchannel [channelId]`");
                return null;
            }
        }
    }
}