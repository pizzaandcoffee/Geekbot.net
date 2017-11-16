using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    [Group("admin")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class Admin : ModuleBase
    {
        private readonly IDatabase _redis;
        private readonly DiscordSocketClient _client;
        private readonly IErrorHandler _errorHandler;
        private readonly ITranslationHandler _translation;
        
        public Admin(IDatabase redis, DiscordSocketClient client, IErrorHandler errorHandler, ITranslationHandler translationHandler)
        {
            _redis = redis;
            _client = client;
            _errorHandler = errorHandler;
            _translation = translationHandler;
        }

        [Command("welcome", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Set a Welcome Message (use '$user' to mention the new joined user).")]
        public async Task SetWelcomeMessage([Remainder] [Summary("message")] string welcomeMessage)
        {
            _redis.HashSet($"{Context.Guild.Id}:Settings", new HashEntry[] { new HashEntry("WelcomeMsg", welcomeMessage) });
            var formatedMessage = welcomeMessage.Replace("$user", Context.User.Mention);
            await ReplyAsync("Welcome message has been changed\r\nHere is an example of how it would look:\r\n" +
                             formatedMessage);
        }

        [Command("modchannel", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Set a channel for moderation purposes")]
        public async Task selectModChannel([Summary("#Channel")] ISocketMessageChannel channel)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Successfully saved mod channel, you can now do the following");
                sb.AppendLine("- `!admin showleave true` - send message to mod channel when someone leaves");
                sb.AppendLine("- `!admin showdel true` - send message to mod channel when someone deletes a message");
                await channel.SendMessageAsync(sb.ToString());
                _redis.HashSet($"{Context.Guild.Id}:Settings", new HashEntry[] {new HashEntry("ModChannel", channel.Id.ToString())});
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context, "That channel doesn't seem to be valid");
            }
        }

        [Command("showleave", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Notify modchannel when someone leaves")]
        public async Task showLeave([Summary("true/false")] bool enabled)
        {
            var modChannelId = ulong.Parse(_redis.HashGet($"{Context.Guild.Id}:Settings", "ModChannel"));
            try
            {
                var modChannel = (ISocketMessageChannel) _client.GetChannel(modChannelId);
                if (enabled)
                {
                    await modChannel.SendMessageAsync("Saved - now sending messages here when someone leaves");
                    _redis.HashSet($"{Context.Guild.Id}:Settings", new HashEntry[] {new HashEntry("ShowLeave", true)});
                }
                else
                {
                    await modChannel.SendMessageAsync("Saved - stopping sending messages here when someone leaves");
                    _redis.HashSet($"{Context.Guild.Id}:Settings", new HashEntry[] {new HashEntry("ShowLeave", false)});
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context, "Modchannel doesn't seem to exist, please set one with `!admin modchannel [channelId]`");
            }
        }
        
        [Command("showdel", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Notify modchannel when someone deletes a message")]
        public async Task showDelete([Summary("true/false")] bool enabled)
        {
            var modChannelId = ulong.Parse(_redis.HashGet($"{Context.Guild.Id}:Settings", "ModChannel"));
            try
            {
                var modChannel = (ISocketMessageChannel) _client.GetChannel(modChannelId);
                if (enabled)
                {
                    await modChannel.SendMessageAsync("Saved - now sending messages here when someone deletes a message");
                    _redis.HashSet($"{Context.Guild.Id}:Settings", new HashEntry[] {new HashEntry("ShowDelete", true)});
                }
                else
                {
                    await modChannel.SendMessageAsync("Saved - stopping sending messages here when someone deletes a message");
                    _redis.HashSet($"{Context.Guild.Id}:Settings", new HashEntry[] {new HashEntry("ShowDelete", false)});
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context, "Modchannel doesn't seem to exist, please set one with `!admin modchannel [channelId]`");
            }
        }
        
        [Command("setlang", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Change the bots language")]
        public async Task setLanguage([Summary("language")] string languageRaw)
        {
            try
            {
                var language = languageRaw.ToUpper();
                var success = _translation.SetLanguage(Context.Guild.Id, language);
                if (success)
                {
                    var trans = _translation.GetDict(Context);
                    await ReplyAsync(trans["NewLanguageSet"]);
                    return;
                }
                await ReplyAsync(
                    $"That doesn't seem to be a supported language\r\nSupported Languages are {string.Join(", ", _translation.GetSupportedLanguages())}");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("lang", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Change the bots language")]
        public async Task getLanguage()
        {
            try
            {
                var trans = _translation.GetDict(Context);
                await ReplyAsync(trans["GetLanguage"]);
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}