using System;
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
        private readonly ILogger _logger;
        private readonly IDatabase _redis;
        private readonly IServiceProvider _servicesProvider;
        private readonly CommandService _commands;
        private readonly IUserRepository _userRepository;
        
        public Handlers(IDiscordClient client,  ILogger logger, IDatabase redis, IServiceProvider servicesProvider, CommandService commands, IUserRepository userRepository)
        {
            _client = client;
            _logger = logger;
            _redis = redis;
            _servicesProvider = servicesProvider;
            _commands = commands;
            _userRepository = userRepository;
        }
        
        //
        // Incoming Messages
        //
        
        public Task RunCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return Task.CompletedTask;
            if (message.Author.IsBot) return Task.CompletedTask;
            var argPos = 0;
            var lowCaseMsg = message.ToString().ToLower();
            if (lowCaseMsg.StartsWith("ping"))
            {
                message.Channel.SendMessageAsync("pong");
                return Task.CompletedTask;
            }
            if (lowCaseMsg.StartsWith("hui"))
            {
                message.Channel.SendMessageAsync("hui!!!");
                return Task.CompletedTask;
            }
            if (!(message.HasCharPrefix('!', ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return Task.CompletedTask;
            var context = new CommandContext(_client, message);
            var commandExec = _commands.ExecuteAsync(context, argPos, _servicesProvider);
            return Task.CompletedTask;
        }

        public Task UpdateStats(SocketMessage messsageParam)
        {
            var message = messsageParam;
            if (message == null) return Task.CompletedTask;
            
            var channel = (SocketGuildChannel) message.Channel;
            
            _redis.HashIncrementAsync($"{channel.Guild.Id}:Messages", message.Author.Id.ToString());
            _redis.HashIncrementAsync($"{channel.Guild.Id}:Messages", 0.ToString());

            if (message.Author.IsBot) return Task.CompletedTask;
            _logger.Information($"[Message] {channel.Guild.Name} - {message.Channel} - {message.Author.Username} - {message.Content}");
            return Task.CompletedTask;
        }
        
        //
        // User Stuff
        //
        
        public Task UserJoined(SocketGuildUser user)
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
            _logger.Information($"[Geekbot] {user.Id} ({user.Username}) joined {user.Guild.Id} ({user.Guild.Name})");
            return Task.CompletedTask;
        }

        public Task UserUpdated(SocketUser oldUser, SocketUser newUser)
        {
            _userRepository.Update(newUser);
            return Task.CompletedTask;
        }
    }
}