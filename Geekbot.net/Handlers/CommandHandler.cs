using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.GuildSettingsManager;
using Geekbot.net.Lib.Logger;

namespace Geekbot.net.Handlers
{
    public class CommandHandler
    {
        private readonly IDiscordClient _client;
        private readonly IGeekbotLogger _logger;
        private readonly IServiceProvider _servicesProvider;
        private readonly CommandService _commands;
        private readonly RestApplication _applicationInfo;
        private readonly IGuildSettingsManager _guildSettingsManager;
        private readonly List<ulong> _ignoredServers;
        private readonly DatabaseContext _database;

        public CommandHandler(DatabaseContext database, IDiscordClient client, IGeekbotLogger logger, IServiceProvider servicesProvider, CommandService commands, RestApplication applicationInfo,
            IGuildSettingsManager guildSettingsManager)
        {
            _database = database;
            _client = client;
            _logger = logger;
            _servicesProvider = servicesProvider;
            _commands = commands;
            _applicationInfo = applicationInfo;
            _guildSettingsManager = guildSettingsManager;

            // Some guilds only want very specific functionally without any of the commands, a quick hack that solves that "short term"
            // ToDo: create a clean solution for this...
            _ignoredServers = new List<ulong>
            {
                228623803201224704, // SwitzerLAN
                169844523181015040, // EEvent
                248531441548263425, // MYI
                110373943822540800 // Discord Bots
            };
        }

        public Task RunCommand(SocketMessage messageParam)
        {
            try
            {
                if (!(messageParam is SocketUserMessage message)) return Task.CompletedTask;
                if (message.Author.IsBot) return Task.CompletedTask;

                ulong guildId = message.Author switch
                {
                    SocketGuildUser user => user.Guild.Id,
                    _ => 0 // DM Channel
                };
                
                if (IsIgnoredGuild(guildId, message.Author.Id)) return Task.CompletedTask;

                var lowCaseMsg = message.ToString().ToLower();
                if (ShouldHui(lowCaseMsg, guildId))
                {
                    message.Channel.SendMessageAsync("hui!!!");
                    return Task.CompletedTask;
                }

                if (ShouldPong(lowCaseMsg, guildId))
                {
                    message.Channel.SendMessageAsync("pong");
                    return Task.CompletedTask;
                }

                var argPos = 0;
                if (!IsCommand(message, ref argPos)) return Task.CompletedTask;

                ExecuteCommand(message, argPos);
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Failed to Process Message", e);
            }

            return Task.CompletedTask;
        }

        private void ExecuteCommand(IUserMessage message, int argPos)
        {
            var context = new CommandContext(_client, message);
            _commands.ExecuteAsync(context, argPos, _servicesProvider);
            _logger.Information(LogSource.Command, context.Message.Content.Split(" ")[0].Replace("!", ""), SimpleConextConverter.ConvertContext(context));
        }

        private bool IsIgnoredGuild(ulong guildId, ulong authorId)
        {
            if (!_ignoredServers.Contains(guildId)) return false;
            return authorId == _applicationInfo.Owner.Id;
        }

        private bool IsCommand(IUserMessage message, ref int argPos)
        {
            return message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos);
        }

        private bool ShouldPong(string lowerCaseMessage, ulong guildId)
        {
            if (!lowerCaseMessage.StartsWith("ping ") && !lowerCaseMessage.Equals("ping")) return false;
            if (guildId == 0) return true;
            return GetGuildSettings(guildId)?.Ping ?? false;
        }

        private bool ShouldHui(string lowerCaseMessage, ulong guildId)
        {
            if (!lowerCaseMessage.StartsWith("hui")) return false;
            if (guildId == 0) return true;
            return GetGuildSettings(guildId)?.Hui ?? false;
        }

        private GuildSettingsModel GetGuildSettings(ulong guildId)
        {
            return _guildSettingsManager.GetSettings(guildId, false);
        }
    }
}