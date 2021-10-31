using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.Core;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.GuildSettingsManager;
using Localization = Geekbot.Core.Localization;

namespace Geekbot.Bot.Commands.Admin
{
    [Group("admin")]
    [RequireUserPermission(GuildPermission.Administrator)]
    [DisableInDirectMessage]
    public class Admin : GeekbotCommandBase
    {
        private readonly DiscordSocketClient _client;

        public Admin(DiscordSocketClient client, IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
            _client = client;
        }

        [Command("welcome", RunMode = RunMode.Async)]
        [Summary("Set a Welcome Message (use '$user' to mention the new joined user).")]
        public async Task SetWelcomeMessage([Remainder, Summary("message")] string welcomeMessage)
        {
            GuildSettings.WelcomeMessage = welcomeMessage;
            await GuildSettingsManager.UpdateSettings(GuildSettings);

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

                GuildSettings.WelcomeChannel = channel.Id.AsLong();
                await GuildSettingsManager.UpdateSettings(GuildSettings);

                await m.DeleteAsync();

                await ReplyAsync("Successfully saved the welcome channel");
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context, "That channel doesn't seem to exist or i don't have write permissions");
            }
        }

        [Command("modchannel", RunMode = RunMode.Async)]
        [Summary("Set a channel for moderation purposes")]
        public async Task SelectModChannel([Summary("#Channel")] ISocketMessageChannel channel)
        {
            try
            {
                var m = await channel.SendMessageAsync("verifying...");

                GuildSettings.ModChannel = channel.Id.AsLong();
                await GuildSettingsManager.UpdateSettings(GuildSettings);

                var sb = new StringBuilder();
                sb.AppendLine("Successfully saved mod channel, you can now do the following");
                sb.AppendLine("- `!admin showleave` - send message to mod channel when someone leaves");
                sb.AppendLine("- `!admin showdel` - send message to mod channel when someone deletes a message");
                await m.ModifyAsync(e => e.Content = sb.ToString());
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context, "That channel doesn't seem to exist or i don't have write permissions");
            }
        }

        [Command("showleave", RunMode = RunMode.Async)]
        [Summary("Toggle - notify modchannel when someone leaves")]
        public async Task ShowLeave()
        {
            try
            {
                var modChannel = await GetModChannel(GuildSettings.ModChannel.AsUlong());
                if (modChannel == null) return;

                GuildSettings.ShowLeave = !GuildSettings.ShowLeave;
                await GuildSettingsManager.UpdateSettings(GuildSettings);
                await modChannel.SendMessageAsync(GuildSettings.ShowLeave
                    ? "Saved - now sending messages here when someone leaves"
                    : "Saved - stopping sending messages here when someone leaves"
                );
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("showdel", RunMode = RunMode.Async)]
        [Summary("Toggle - notify modchannel when someone deletes a message")]
        public async Task ShowDelete()
        {
            try
            {
                var modChannel = await GetModChannel(GuildSettings.ModChannel.AsUlong());
                if (modChannel == null) return;

                GuildSettings.ShowDelete = !GuildSettings.ShowDelete;
                await GuildSettingsManager.UpdateSettings(GuildSettings);
                await modChannel.SendMessageAsync(GuildSettings.ShowDelete
                    ? "Saved - now sending messages here when someone deletes a message"
                    : "Saved - stopping sending messages here when someone deletes a message"
                );
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("setlang", RunMode = RunMode.Async)]
        [Summary("Change the bots language")]
        public async Task SetLanguage([Summary("language")] string language)
        {
            try
            {
                var availableLanguages = new List<string>();
                availableLanguages.Add("en-GB"); // default
                availableLanguages.AddRange(GetAvailableCultures().Select(culture => culture.Name));
                if (availableLanguages.Contains(language))
                {
                    GuildSettings.Language = language;
                    await GuildSettingsManager.UpdateSettings(GuildSettings);

                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(language.ToLower() == "chde" ? "de-CH" : language);

                    await ReplyAsync(Localization.Admin.NewLanguageSet);
                    return;
                }
                
                await ReplyAsync($"That doesn't seem to be a supported language\nSupported Languages are {string.Join(", ", availableLanguages)}");
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("wiki", RunMode = RunMode.Async)]
        [Summary("Change the wikipedia instance (use lang code in xx.wikipedia.org)")]
        public async Task SetWikiLanguage([Summary("language")] string languageRaw)
        {
            try
            {
                var language = languageRaw.ToLower();
                GuildSettings.WikiLang = language;
                await GuildSettingsManager.UpdateSettings(GuildSettings);

                await ReplyAsync($"Now using the {language} wikipedia");
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("ping", RunMode = RunMode.Async)]
        [Summary("Enable the ping reply.")]
        public async Task TogglePing()
        {
            try
            {
                // var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                GuildSettings.Ping = !GuildSettings.Ping;
                await GuildSettingsManager.UpdateSettings(GuildSettings);
                await ReplyAsync(GuildSettings.Ping ? "i will reply to ping now" : "No more pongs...");
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("hui", RunMode = RunMode.Async)]
        [Summary("Enable the ping reply.")]
        public async Task ToggleHui()
        {
            try
            {
                // var guild = _guildSettingsManager.GetSettings(Context.Guild.Id);
                GuildSettings.Hui = !GuildSettings.Hui;
                await GuildSettingsManager.UpdateSettings(GuildSettings);
                await ReplyAsync(GuildSettings.Hui ? "i will reply to hui now" : "No more hui's...");
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
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
        
        private IEnumerable<CultureInfo> GetAvailableCultures()
        {
            var result = new List<CultureInfo>();
            var rm = new ResourceManager(typeof(Localization.Admin));
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture)) continue; //do not use "==", won't work

                    var rs = rm.GetResourceSet(culture, true, false);
                    if (rs != null)
                    {
                        result.Add(culture);
                    }
                }
                catch (CultureNotFoundException)
                {
                    //NOP
                }
            }
            return result;
        }
    }
}