using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.CommandPreconditions;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
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
        private readonly DatabaseContext _database;
        private readonly ITranslationHandler _translation;

        public Admin(DatabaseContext database, DiscordSocketClient client, IErrorHandler errorHandler,
            ITranslationHandler translationHandler)
        {
            _database = database;
            _client = client;
            _errorHandler = errorHandler;
            _translation = translationHandler;
        }

        [Command("welcome", RunMode = RunMode.Async)]
        [Summary("Set a Welcome Message (use '$user' to mention the new joined user).")]
        public async Task SetWelcomeMessage([Remainder] [Summary("message")] string welcomeMessage)
        {
            var guild = await GetGuildSettings(Context.Guild.Id);
            guild.WelcomeMessage = welcomeMessage;
            _database.GuildSettings.Update(guild);
            await _database.SaveChangesAsync();
            
            var formatedMessage = welcomeMessage.Replace("$user", Context.User.Mention);
            await ReplyAsync($"Welcome message has been changed\r\nHere is an example of how it would look:\r\n{formatedMessage}");
        }

        [Command("modchannel", RunMode = RunMode.Async)]
        [Summary("Set a channel for moderation purposes")]
        public async Task SelectModChannel([Summary("#Channel")] ISocketMessageChannel channel)
        {
            try
            {
                var m = await channel.SendMessageAsync("verifying...");

                var guild = await GetGuildSettings(Context.Guild.Id);
                guild.ModChannel = channel.Id.AsLong();
                _database.GuildSettings.Update(guild);
                await _database.SaveChangesAsync();
                
                var sb = new StringBuilder();
                sb.AppendLine("Successfully saved mod channel, you can now do the following");
                sb.AppendLine("- `!admin showleave` - send message to mod channel when someone leaves");
                sb.AppendLine("- `!admin showdel` - send message to mod channel when someone deletes a message");
                await m.ModifyAsync(e => e.Content = sb.ToString());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context, "That channel doesn't seem to be valid");
            }
        }

        [Command("showleave", RunMode = RunMode.Async)]
        [Summary("Toggle - notify modchannel when someone leaves")]
        public async Task ShowLeave()
        {
            try
            {
                var guild = await GetGuildSettings(Context.Guild.Id);
                var modChannel = await GetModChannel(guild.ModChannel.AsUlong());
                if (modChannel == null) return;
                
                guild.ShowLeave = !guild.ShowLeave;
                _database.GuildSettings.Update(guild);
                await _database.SaveChangesAsync();
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
                var guild = await GetGuildSettings(Context.Guild.Id);
                var modChannel = await GetModChannel(guild.ModChannel.AsUlong());
                if (modChannel == null) return;
                
                guild.ShowDelete = !guild.ShowDelete;
                _database.GuildSettings.Update(guild);
                await _database.SaveChangesAsync();
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
                    var guild = await GetGuildSettings(Context.Guild.Id);
                    guild.Language = language;
                    _database.GuildSettings.Update(guild);
                    await _database.SaveChangesAsync();
                    
                    var trans = await _translation.GetDict(Context);
                    await ReplyAsync(trans["NewLanguageSet"]);
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
                var guild = await GetGuildSettings(Context.Guild.Id);
                guild.WikiLang = language;
                _database.GuildSettings.Update(guild);
                await _database.SaveChangesAsync();
                
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
                var guild = await GetGuildSettings(Context.Guild.Id);
                guild.Ping = !guild.Ping;
                _database.GuildSettings.Update(guild);
                await _database.SaveChangesAsync();
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
                var guild = await GetGuildSettings(Context.Guild.Id);
                guild.Hui = !guild.Hui;
                _database.GuildSettings.Update(guild);
                await _database.SaveChangesAsync();
                await ReplyAsync(guild.Hui ? "i will reply to hui now" : "No more hui's...");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task<GuildSettingsModel> GetGuildSettings(ulong guildId)
        {
            var guild = _database.GuildSettings.FirstOrDefault(g => g.GuildId.Equals(guildId.AsLong()));
            if (guild != null) return guild;
            Console.WriteLine("Adding non-exist Guild Settings to database");
            _database.GuildSettings.Add(new GuildSettingsModel()
            {
                GuildId = guildId.AsLong(),
                Hui = false,
                Ping = false,
                Language = "EN",
                ShowDelete = false,
                ShowLeave = false,
                WikiLang = "en"
            });
            await _database.SaveChangesAsync();
            return _database.GuildSettings.FirstOrDefault(g => g.GuildId.Equals(guildId.AsLong()));
        }

        private async Task<ISocketMessageChannel> GetModChannel(ulong channelId)
        {
            try
            {
                if(channelId == ulong.MinValue) throw new Exception();
                var modChannel = (ISocketMessageChannel) _client.GetChannel(channelId);
                if(modChannel == null) throw new Exception();
                return modChannel;
            }
            catch
            {
                await ReplyAsync(
                    "Modchannel doesn't seem to exist, please set one with `!admin modchannel [channelId]`");
                return null;
            }
        }
        
    }
}