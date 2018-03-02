﻿using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net
{
    public class Handlers
    {
        private readonly IDiscordClient _client;
        private readonly IGeekbotLogger _logger;
        private readonly IDatabase _redis;
        private readonly IServiceProvider _servicesProvider;
        private readonly CommandService _commands;
        private readonly IUserRepository _userRepository;
        private readonly IReactionListener _reactionListener;
        
        public Handlers(IDiscordClient client,  IGeekbotLogger logger, IDatabase redis, IServiceProvider servicesProvider, CommandService commands, IUserRepository userRepository, IReactionListener reactionListener)
        {
            _client = client;
            _logger = logger;
            _redis = redis;
            _servicesProvider = servicesProvider;
            _commands = commands;
            _userRepository = userRepository;
            _reactionListener = reactionListener;
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
                var lowCaseMsg = message.ToString().ToLower();
                if (lowCaseMsg.StartsWith("hui"))
                {
                    message.Channel.SendMessageAsync("hui!!!");
                    return Task.CompletedTask;
                }
                if (lowCaseMsg.StartsWith("ping ") || lowCaseMsg.Equals("ping"))
                {
                    bool.TryParse(_redis.HashGet($"{((SocketGuildChannel) message.Channel).Guild.Id}:Settings", "ping"), out var allowPings);
                    if (allowPings)
                    {
                        message.Channel.SendMessageAsync("pong");
                        return Task.CompletedTask;  
                    }
                }
                if (!(message.HasCharPrefix('!', ref argPos) ||
                      message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return Task.CompletedTask;
                var context = new CommandContext(_client, message);
                var commandExec = _commands.ExecuteAsync(context, argPos, _servicesProvider);
                _logger.Information("Command",
                    context.Message.Content.Split(" ")[0].Replace("!", ""),
                    SimpleConextConverter.ConvertContext(context));
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.Error("Geekbot", "Failed to run commands", e);
                return Task.CompletedTask;
            }
        }

        public Task UpdateStats(SocketMessage message)
        {
            try
            {
                if (message == null) return Task.CompletedTask;
                if (message.Channel.Name.StartsWith('@'))
                {
                    _logger.Information("Message", "DM-Channel - {message.Channel.Name} - {message.Content}");
                    return Task.CompletedTask;
                }
                var channel = (SocketGuildChannel) message.Channel;

                _redis.HashIncrementAsync($"{channel.Guild.Id}:Messages", message.Author.Id.ToString());
                _redis.HashIncrementAsync($"{channel.Guild.Id}:Messages", 0.ToString());

                if (message.Author.IsBot) return Task.CompletedTask;
                _logger.Information("Message", message.Content, SimpleConextConverter.ConvertSocketMessage(message));
//                _logger.Information($"[Message] {channel.Guild.Name} ({channel.Guild.Id}) - {message.Channel} ({message.Channel.Id}) - {message.Author.Username}#{message.Author.Discriminator} ({message.Author.Id}) - {message.Content}");
            }
            catch (Exception e)
            {
                _logger.Error("Message", "Could not process message stats", e);
            }
            return Task.CompletedTask;
        }
        
        //
        // User Stuff
        //
        
        public Task UserJoined(SocketGuildUser user)
        {
            try
            {
                if (!user.IsBot)
                {
                    var message = _redis.HashGet($"{user.Guild.Id}:Settings", "WelcomeMsg");
                    if (!message.IsNullOrEmpty)
                    {
                        message = message.ToString().Replace("$user", user.Mention);
                        user.Guild.DefaultChannel.SendMessageAsync(message);
                    }
                }
                _userRepository.Update(user);
                _logger.Information("Geekbot", $"{user.Username} ({user.Id}) joined {user.Guild.Name} ({user.Guild.Id})");
            }
            catch (Exception e)
            {
                _logger.Error("Geekbot", "Failed to send welcome message", e);
            }
            return Task.CompletedTask;
        }

        public Task UserUpdated(SocketUser oldUser, SocketUser newUser)
        {
            _userRepository.Update(newUser);
            return Task.CompletedTask;
        }

        public async Task UserLeft(SocketGuildUser user)
        {
            try
            {
                var sendLeftEnabled = _redis.HashGet($"{user.Guild.Id}:Settings", "ShowLeave");
                if (sendLeftEnabled.ToString() == "1")
                {
                    var modChannel = ulong.Parse(_redis.HashGet($"{user.Guild.Id}:Settings", "ModChannel"));
                    if (!string.IsNullOrEmpty(modChannel.ToString()))
                    {
                        var modChannelSocket = (ISocketMessageChannel) await _client.GetChannelAsync(modChannel);
                        await modChannelSocket.SendMessageAsync($"{user.Username}#{user.Discriminator} left the server");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Geekbot", "Failed to send leave message", e);
            }
            _logger.Information("Geekbot", $"{user.Username} ({user.Id}) joined {user.Guild.Name} ({user.Guild.Id})");
        }
        
        //
        // Message Stuff
        //
        
        public async Task MessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            try
            {
                var guild = ((IGuildChannel) channel).Guild;
                var sendLeftEnabled = _redis.HashGet($"{guild.Id}:Settings", "ShowDelete");
                if (sendLeftEnabled.ToString() == "1")
                {
                    var modChannel = ulong.Parse(_redis.HashGet($"{guild.Id}:Settings", "ModChannel"));
                    if (!string.IsNullOrEmpty(modChannel.ToString()) && modChannel != channel.Id)
                    {
                        var modChannelSocket = (ISocketMessageChannel) await _client.GetChannelAsync(modChannel);
                        var sb = new StringBuilder();
                        if (message.Value != null)
                        {
                            sb.AppendLine(
                                $"The following message from {message.Value.Author.Username}#{message.Value.Author.Discriminator} was deleted in <#{channel.Id}>");
                            sb.AppendLine(message.Value.Content);
                        }
                        else
                        {
                            sb.AppendLine("Someone deleted a message, the message was not cached...");
                        }
                        await modChannelSocket.SendMessageAsync(sb.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Geekbot", "Failed to send delete message...", e);
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
