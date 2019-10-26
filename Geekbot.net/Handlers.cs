using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.AlmostRedis;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Logger;
using Geekbot.net.Lib.ReactionListener;
using Geekbot.net.Lib.UserRepository;
using Microsoft.EntityFrameworkCore;

namespace Geekbot.net
{
    public class Handlers
    {
        private readonly DatabaseContext _database;
        private readonly IDiscordClient _client;
        private readonly IGeekbotLogger _logger;
        private readonly IAlmostRedis _redis;
        private readonly IServiceProvider _servicesProvider;
        private readonly CommandService _commands;
        private readonly IUserRepository _userRepository;
        private readonly IReactionListener _reactionListener;
        private readonly DatabaseContext _messageCounterDatabaseContext;
        private readonly RestApplication _applicationInfo;
        private readonly List<ulong> _ignoredServers;

        public Handlers(DatabaseInitializer databaseInitializer, IDiscordClient client, IGeekbotLogger logger, IAlmostRedis redis,
            IServiceProvider servicesProvider, CommandService commands, IUserRepository userRepository,
            IReactionListener reactionListener, RestApplication applicationInfo)
        {
            _database = databaseInitializer.Initialize();
            _messageCounterDatabaseContext = databaseInitializer.Initialize();
            _client = client;
            _logger = logger;
            _redis = redis;
            _servicesProvider = servicesProvider;
            _commands = commands;
            _userRepository = userRepository;
            _reactionListener = reactionListener;
            _applicationInfo = applicationInfo;
            // ToDo: create a clean solution for this...
            _ignoredServers = new List<ulong>()
            {
                228623803201224704, // SwitzerLAN
                169844523181015040, // EEvent
                248531441548263425, // MYI
                110373943822540800  // Discord Bots
            };
        }

        //
        // Incoming Messages
        //

        public Task RunCommand(SocketMessage messageParam)
        {
            try
            {
                if (!(messageParam is SocketUserMessage message)) return Task.CompletedTask;
                if (message.Author.IsBot) return Task.CompletedTask;
                var argPos = 0;

                var guildId = ((SocketGuildChannel) message.Channel).Guild.Id;
                // Some guilds only wanted very specific functionally without any of the commands, a quick hack that solves that short term...
                // ToDo: cleanup
                if (_ignoredServers.Contains(guildId))
                {
                    if (message.Author.Id != _applicationInfo.Owner.Id)
                    {
                        return Task.CompletedTask;
                    }
                }
                
                var lowCaseMsg = message.ToString().ToLower();
                if (lowCaseMsg.StartsWith("hui"))
                {
                    var hasPing = _database.GuildSettings.FirstOrDefault(guild => guild.GuildId.Equals(guildId.AsLong()))?.Hui ?? false;
                    if (hasPing)
                    {
                        message.Channel.SendMessageAsync("hui!!!");
                        return Task.CompletedTask;
                    }
                }

                if (lowCaseMsg.StartsWith("ping ") || lowCaseMsg.Equals("ping"))
                {
                    var hasPing = _database.GuildSettings.FirstOrDefault(guild => guild.GuildId.Equals(guildId.AsLong()))?.Ping ?? false;
                    if (hasPing)
                    {
                        message.Channel.SendMessageAsync("pong");
                        return Task.CompletedTask;
                    }
                }

                if (!(message.HasCharPrefix('!', ref argPos) ||
                      message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return Task.CompletedTask;
                var context = new CommandContext(_client, message);
                var commandExec = _commands.ExecuteAsync(context, argPos, _servicesProvider);
                _logger.Information(LogSource.Command,
                    context.Message.Content.Split(" ")[0].Replace("!", ""),
                    SimpleConextConverter.ConvertContext(context));
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Failed to Process Message", e);
                return Task.CompletedTask;
            }
        }

        public async Task UpdateStats(SocketMessage message)
        {
            try
            {
                if (message == null) return;
                if (message.Channel.Name.StartsWith('@'))
                {
                    _logger.Information(LogSource.Message, $"[DM-Channel] {message.Content}", SimpleConextConverter.ConvertSocketMessage(message, true));
                    return;
                }

                var channel = (SocketGuildChannel) message.Channel;

                var rowId = await _messageCounterDatabaseContext.Database.ExecuteSqlRawAsync(
                    "UPDATE \"Messages\" SET \"MessageCount\" = \"MessageCount\" + 1 WHERE \"GuildId\" = {0} AND \"UserId\" = {1}",
                    channel.Guild.Id.AsLong(),
                    message.Author.Id.AsLong()
                    );

                if (rowId == 0)
                {
                    _messageCounterDatabaseContext.Messages.Add(new MessagesModel
                    {
                        UserId = message.Author.Id.AsLong(),
                        GuildId = channel.Guild.Id.AsLong(),
                        MessageCount = 1
                    });
                    _messageCounterDatabaseContext.SaveChanges();
                }

                if (message.Author.IsBot) return;
                _logger.Information(LogSource.Message, message.Content, SimpleConextConverter.ConvertSocketMessage(message));
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Message, "Could not process message stats", e);
            }
        }

        //
        // User Stuff
        //

        public async Task UserJoined(SocketGuildUser user)
        {
            try
            {
                var userRepoUpdate = _userRepository.Update(user);
                _logger.Information(LogSource.Geekbot, $"{user.Username} ({user.Id}) joined {user.Guild.Name} ({user.Guild.Id})");

                if (!user.IsBot)
                {
                    var guildSettings = _database.GuildSettings.FirstOrDefault(guild => guild.GuildId == user.Guild.Id.AsLong());
                    var message = guildSettings?.WelcomeMessage;
                    if (string.IsNullOrEmpty(message)) return;
                    message = message.Replace("$user", user.Mention);

                    var fallbackSender = new Func<Task<RestUserMessage>>(() => user.Guild.DefaultChannel.SendMessageAsync(message));
                    if (guildSettings.WelcomeChannel != 0)
                    {
                        try
                        {
                            var target = await _client.GetChannelAsync(guildSettings.WelcomeChannel.AsUlong());
                            var channel = target as ISocketMessageChannel;
                            await channel.SendMessageAsync(message);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(LogSource.Geekbot, "Failed to send welcome message to user defined welcome channel", e);
                            await fallbackSender();
                        }
                    }
                    else
                    {
                        await fallbackSender();
                    }
                }

                await userRepoUpdate;
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Failed to send welcome message", e);
            }
        }

        public async Task UserUpdated(SocketUser oldUser, SocketUser newUser)
        {
            await _userRepository.Update(newUser);
        }

        public async Task UserLeft(SocketGuildUser user)
        {
            try
            {
                var guild = _database.GuildSettings.FirstOrDefault(g =>
                    g.GuildId.Equals(user.Guild.Id.AsLong()));
                if (guild?.ShowLeave ?? false)
                {
                    var modChannelSocket = (ISocketMessageChannel) await _client.GetChannelAsync(guild.ModChannel.AsUlong());
                    await modChannelSocket.SendMessageAsync($"{user.Username}#{user.Discriminator} left the server");
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Failed to send leave message", e);
            }

            _logger.Information(LogSource.Geekbot, $"{user.Username} ({user.Id}) joined {user.Guild.Name} ({user.Guild.Id})");
        }

        //
        // Message Stuff
        //

        public async Task MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            try
            {
                var guildSocketData = ((IGuildChannel) channel).Guild;
                var guild = _database.GuildSettings.FirstOrDefault(g => g.GuildId.Equals(guildSocketData.Id.AsLong()));
                if ((guild?.ShowDelete ?? false) && guild?.ModChannel != 0)
                {
                    var modChannelSocket = (ISocketMessageChannel) await _client.GetChannelAsync(guild.ModChannel.AsUlong());
                    var sb = new StringBuilder();
                    if (message.Value != null)
                    {
                        sb.AppendLine($"The following message from {message.Value.Author.Username}#{message.Value.Author.Discriminator} was deleted in <#{channel.Id}>");
                        sb.AppendLine(message.Value.Content);
                    }
                    else
                    {
                        sb.AppendLine("Someone deleted a message, the message was not cached...");
                    }

                    await modChannelSocket.SendMessageAsync(sb.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Failed to send delete message...", e);
            }
        }

        //
        // Reactions
        // 

        public Task ReactionAdded(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return Task.CompletedTask;
            if (!_reactionListener.IsListener(reaction.MessageId)) return Task.CompletedTask;
            _reactionListener.GiveRole(socketMessageChannel, reaction);
            return Task.CompletedTask;
        }

        public Task ReactionRemoved(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return Task.CompletedTask;
            if (!_reactionListener.IsListener(reaction.MessageId)) return Task.CompletedTask;
            _reactionListener.RemoveRole(socketMessageChannel, reaction);
            return Task.CompletedTask;
        }
    }
}