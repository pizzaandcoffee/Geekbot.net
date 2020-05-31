using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib.AlmostRedis;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.GlobalSettings;
using Geekbot.net.Lib.Logger;
using Geekbot.net.Lib.ReactionListener;
using Geekbot.net.Lib.UserRepository;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Admin.Owner
{
    [Group("owner")]
    [RequireOwner]
    public class Owner : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly IErrorHandler _errorHandler;
        private readonly IGlobalSettings _globalSettings;
        private readonly IDatabase _redis;
        private readonly IReactionListener _reactionListener;
        private readonly IGeekbotLogger _logger;
        private readonly IUserRepository _userRepository;

        public Owner(DiscordSocketClient client, IGeekbotLogger logger, IUserRepository userRepositry, IErrorHandler errorHandler, IGlobalSettings globalSettings,
            IAlmostRedis redis, IReactionListener reactionListener
        )
        {
            _client = client;
            _logger = logger;
            _userRepository = userRepositry;
            _errorHandler = errorHandler;
            _globalSettings = globalSettings;
            _redis = redis.Db;
            _reactionListener = reactionListener;
        }

        [Command("youtubekey", RunMode = RunMode.Async)]
        [Summary("Set the youtube api key")]
        public async Task SetYoutubeKey([Summary("API-Key")] string key)
        {
            await _globalSettings.SetKey("YoutubeKey", key);
            await ReplyAsync("Apikey has been set");
        }

        [Command("game", RunMode = RunMode.Async)]
        [Summary("Set the game that the bot is playing")]
        public async Task SetGame([Remainder] [Summary("Game")] string key)
        {
            await _globalSettings.SetKey("Game", key);
            await _client.SetGameAsync(key);
            _logger.Information(LogSource.Geekbot, $"Changed game to {key}");
            await ReplyAsync($"Now Playing {key}");
        }

        [Command("popuserrepo", RunMode = RunMode.Async)]
        [Summary("Populate user cache")]
        public async Task PopUserRepoCommand()
        {
            var success = 0;
            var failed = 0;
            try
            {
                _logger.Warning(LogSource.UserRepository, "Populating User Repositry");
                await ReplyAsync("Starting Population of User Repository");
                foreach (var guild in _client.Guilds)
                {
                    _logger.Information(LogSource.UserRepository, $"Populating users from {guild.Name}");
                    foreach (var user in guild.Users)
                    {
                        var succeded = await _userRepository.Update(user);
                        var inc = succeded ? success++ : failed++;
                    }
                }

                _logger.Warning(LogSource.UserRepository, "Finished Updating User Repositry");
                await ReplyAsync(
                    $"Successfully Populated User Repository with {success} Users in {_client.Guilds.Count} Guilds (Failed: {failed})");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context,
                    "Couldn't complete User Repository, see console for more info");
            }
        }

        [Command("refreshuser", RunMode = RunMode.Async)]
        [Summary("Refresh a user in the user cache")]
        public async Task PopUserRepoCommand([Summary("@someone")] IUser user)
        {
            try
            {
                await _userRepository.Update(user as SocketUser);
                await ReplyAsync($"Refreshed: {user.Username}#{user.Discriminator}");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("refreshuser", RunMode = RunMode.Async)]
        [Summary("Refresh a user in the user cache")]
        public async Task PopUserRepoCommand([Summary("user-id")] ulong userId)
        {
            try
            {
                var user = _client.GetUser(userId);
                await _userRepository.Update(user);
                await ReplyAsync($"Refreshed: {user.Username}#{user.Discriminator}");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("error", RunMode = RunMode.Async)]
        [Summary("Throw an error un purpose")]
        public async Task PurposefulError()
        {
            try
            {
                throw new Exception("Error Generated by !owner error");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
       
        [Command("migrate_listeners", RunMode = RunMode.Async)]
        public async Task MigrateListeners()
        {
            try
            {
                var messageIds = _redis.SetMembers("MessageIds");
                var connectedGuilds = _client.Guilds;
                var messageGuildAssociation = new Dictionary<ulong, ulong>();
                foreach (var messageIdUnparsed in messageIds)
                {
                    var messageId = ulong.Parse(messageIdUnparsed);
                    var reactions = _redis.HashGetAll($"Messages:{messageIdUnparsed.ToString()}");

                    foreach (var reaction in reactions)
                    {
                        _logger.Information(LogSource.Migration, $"{messageIdUnparsed.ToString()} - Starting");
                        try
                        {
                            ulong guildId = 0;
                            IRole role = null;

                            var roleId = ulong.Parse(reaction.Value);
                            if (messageGuildAssociation.ContainsKey(messageId))
                            {
                                guildId = messageGuildAssociation[messageId];
                                _logger.Information(LogSource.Migration, $"{messageIdUnparsed.ToString()} - known to be in {guildId}");
                                role = _client.GetGuild(guildId).GetRole(roleId);
                            }
                            else
                            {
                                _logger.Information(LogSource.Migration, $"{messageIdUnparsed.ToString()} - Attempting to find guild");
                                IRole foundRole = null;
                                try
                                {
                                    foreach (var guild in connectedGuilds)
                                    {
                                        foundRole = guild.GetRole(roleId);
                                        if (foundRole != null)
                                        {
                                            role = _client.GetGuild(foundRole.Guild.Id).GetRole(ulong.Parse(reaction.Value));
                                            messageGuildAssociation.Add(messageId, foundRole.Guild.Id);
                                        }
                                    }
                                } catch { /* ignore */ }

                                if (foundRole == null)
                                {
                                    _logger.Warning(LogSource.Migration, $"{messageIdUnparsed.ToString()} - Could not find guild for message");
                                    continue;
                                }
                            }
                            _logger.Information(LogSource.Migration, $"{messageIdUnparsed.ToString()} - Found Role {roleId.ToString()}");
                            await _reactionListener.AddRoleToListener(ulong.Parse(messageIdUnparsed), guildId, reaction.Name, role);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(LogSource.Migration, $"Failed to migrate reaction for {messageIdUnparsed.ToString()}", e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}