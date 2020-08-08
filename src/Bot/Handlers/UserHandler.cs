using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Geekbot.Core.Database;
using Geekbot.Core.Extensions;
using Geekbot.Core.Logger;
using Geekbot.Core.UserRepository;

namespace Geekbot.Bot.Handlers
{
    public class UserHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IGeekbotLogger _logger;
        private readonly DatabaseContext _database;
        private readonly IDiscordClient _client;

        public UserHandler(IUserRepository userRepository, IGeekbotLogger logger, DatabaseContext database, IDiscordClient client)
        {
            _userRepository = userRepository;
            _logger = logger;
            _database = database;
            _client = client;
        }
        
        public async Task Joined(SocketGuildUser user)
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

        public async Task Updated(SocketUser oldUser, SocketUser newUser)
        {
            await _userRepository.Update(newUser);
        }

        public async Task Left(SocketGuildUser user)
        {
            try
            {
                var guild = _database.GuildSettings.FirstOrDefault(g => g.GuildId.Equals(user.Guild.Id.AsLong()));
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
    }
}