﻿using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Localization;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Admin
{
    [Group("admin")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class Admin : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;
        private readonly ITranslationHandler _translation;

        public Admin(IDatabase redis, DiscordSocketClient client, IErrorHandler errorHandler,
            ITranslationHandler translationHandler)
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
            _redis.HashSet($"{Context.Guild.Id}:Settings", new[] {new HashEntry("WelcomeMsg", welcomeMessage)});
            var formatedMessage = welcomeMessage.Replace("$user", Context.User.Mention);
            await ReplyAsync($"Welcome message has been changed\r\nHere is an example of how it would look:\r\n{formatedMessage}");
        }

        [Command("modchannel", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Set a channel for moderation purposes")]
        public async Task SelectModChannel([Summary("#Channel")] ISocketMessageChannel channel)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Successfully saved mod channel, you can now do the following");
                sb.AppendLine("- `!admin showleave true` - send message to mod channel when someone leaves");
                sb.AppendLine("- `!admin showdel true` - send message to mod channel when someone deletes a message");
                await channel.SendMessageAsync(sb.ToString());
                _redis.HashSet($"{Context.Guild.Id}:Settings",
                    new[] {new HashEntry("ModChannel", channel.Id.ToString())});
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context, "That channel doesn't seem to be valid");
            }
        }

        [Command("showleave", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Notify modchannel when someone leaves")]
        public async Task ShowLeave([Summary("true/false")] bool enabled)
        {
            var modChannelId = ulong.Parse(_redis.HashGet($"{Context.Guild.Id}:Settings", "ModChannel"));
            try
            {
                var modChannel = (ISocketMessageChannel) _client.GetChannel(modChannelId);
                if (enabled)
                {
                    await modChannel.SendMessageAsync("Saved - now sending messages here when someone leaves");
                    _redis.HashSet($"{Context.Guild.Id}:Settings", new[] {new HashEntry("ShowLeave", true)});
                }
                else
                {
                    await modChannel.SendMessageAsync("Saved - stopping sending messages here when someone leaves");
                    _redis.HashSet($"{Context.Guild.Id}:Settings", new[] {new HashEntry("ShowLeave", false)});
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context,
                    "Modchannel doesn't seem to exist, please set one with `!admin modchannel [channelId]`");
            }
        }

        [Command("showdel", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Notify modchannel when someone deletes a message")]
        public async Task ShowDelete([Summary("true/false")] bool enabled)
        {
            var modChannelId = ulong.Parse(_redis.HashGet($"{Context.Guild.Id}:Settings", "ModChannel"));
            try
            {
                var modChannel = (ISocketMessageChannel) _client.GetChannel(modChannelId);
                if (enabled)
                {
                    await modChannel.SendMessageAsync(
                        "Saved - now sending messages here when someone deletes a message");
                    _redis.HashSet($"{Context.Guild.Id}:Settings", new[] {new HashEntry("ShowDelete", true)});
                }
                else
                {
                    await modChannel.SendMessageAsync(
                        "Saved - stopping sending messages here when someone deletes a message");
                    _redis.HashSet($"{Context.Guild.Id}:Settings", new[] {new HashEntry("ShowDelete", false)});
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context,
                    "Modchannel doesn't seem to exist, please set one with `!admin modchannel [channelId]`");
            }
        }

        [Command("setlang", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Change the bots language")]
        public async Task SetLanguage([Summary("language")] string languageRaw)
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
        
        [Command("wiki", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Change the wikipedia instance (use lang code in xx.wikipedia.org)")]
        public async Task SetWikiLanguage([Summary("language")] string languageRaw)
        {
            try
            {
                var language = languageRaw.ToLower();
                _redis.HashSet($"{Context.Guild.Id}:Settings", new[] {new HashEntry("WikiLang", language) });

                await ReplyAsync($"Now using the {language} wikipedia");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("lang", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Change the bots language")]
        public async Task GetLanguage()
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
        
        [Command("ping", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Enable the ping reply.")]
        public async Task TogglePing()
        {
            try
            {
                bool.TryParse(_redis.HashGet($"{Context.Guild.Id}:Settings", "ping"), out var current);
                _redis.HashSet($"{Context.Guild.Id}:Settings", new[] {new HashEntry("ping", current ? "false" : "true") });
                await ReplyAsync(!current ? "i will reply to ping now" : "No more pongs...");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
        
    }
}