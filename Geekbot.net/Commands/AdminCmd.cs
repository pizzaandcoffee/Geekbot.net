using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    [Group("admin")]
    public class AdminCmd : ModuleBase
    {
        private readonly IDatabase _redis;
        private readonly DiscordSocketClient _client;
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        private readonly IErrorHandler _errorHandler;
        
        public AdminCmd(IDatabase redis, DiscordSocketClient client, ILogger logger, IUserRepository userRepositry, IErrorHandler errorHandler)
        {
            _redis = redis;
            _client = client;
            _logger = logger;
            _userRepository = userRepositry;
            _errorHandler = errorHandler;
        }

        [RequireUserPermission(GuildPermission.Administrator)]
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

        [Command("youtubekey", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Set the youtube api key")]
        public async Task SetYoutubeKey([Summary("API Key")] string key)
        {
            var botOwner = Context.Guild.GetUserAsync(ulong.Parse(_redis.StringGet("botOwner"))).Result;
            if (!Context.User.Id.ToString().Equals(botOwner.Id.ToString()))
            {
                await ReplyAsync($"Sorry, only the botowner can do this ({botOwner.Username}#{botOwner.Discriminator})");
                return;
            }

            _redis.StringSet("youtubeKey", key);
            await ReplyAsync("Apikey has been set");
        }
        
        [Command("game", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Set the game that the bot is playing")]
        public async Task SetGame([Remainder] [Summary("Game")] string key)
        {
            var botOwner = Context.Guild.GetUserAsync(ulong.Parse(_redis.StringGet("botOwner"))).Result;
            if (!Context.User.Id.ToString().Equals(botOwner.Id.ToString()))
            {
                await ReplyAsync($"Sorry, only the botowner can do this ({botOwner.Username}#{botOwner.Discriminator})");
                return;
            }

            _redis.StringSet("Game", key);
            await _client.SetGameAsync(key);
            _logger.Information($"[Geekbot] Changed game to {key}");
            await ReplyAsync($"Now Playing {key}");
        }
        
        [Command("popuserrepo", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Populate user cache")]
        public async Task popUserRepoCommand()
        {
            try
            {
                var botOwner = Context.Guild.GetUserAsync(ulong.Parse(_redis.StringGet("botOwner"))).Result;
                if (!Context.User.Id.ToString().Equals(botOwner.Id.ToString()))
                {
                    await ReplyAsync(
                        $"Sorry, only the botowner can do this ({botOwner.Username}#{botOwner.Discriminator})");
                    return;
                }
            }
            catch (Exception)
            {
                await ReplyAsync(
                    $"Sorry, only the botowner can do this");
                return;
            }
            var success = 0;
            var failed = 0;
            try
            {
                _logger.Warning("[UserRepository] Populating User Repositry");
                await ReplyAsync("Starting Population of User Repository");
                foreach (var guild in _client.Guilds)
                {
                    _logger.Information($"[UserRepository] Populating users from {guild.Name}");
                    foreach (var user in guild.Users)
                    {
                        var succeded = await _userRepository.Update(user);
                        var inc = succeded ? success++ : failed++;
                    }
                }
                _logger.Warning("[UserRepository] Finished Updating User Repositry");
                await ReplyAsync($"Successfully Populated User Repository with {success} Users in {_client.Guilds.Count} Guilds (Failed: {failed})");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context, "Couldn't complete User Repository, see console for more info");
            }
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("modchannel", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Set a channel for moderation purposes")]
        public async Task selectModChannel([Summary("ChannelId")] ulong channelId)
        {
            try
            {
                var channel = (ISocketMessageChannel)_client.GetChannel(channelId);
                if (string.IsNullOrEmpty(channel.Name))
                {
                    await ReplyAsync("I couldn't find that channel...");
                    return;
                }
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

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("showleave", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Notify modchannel when someone leaves")]
        public async Task showLeave([Summary("true/false")] bool enabled)
        {
            var modChannelId = (ulong)_redis.HashGet($"{Context.Guild.Id}:Settings", "ModChannel");
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
        
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("showdel", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Notify modchannel when someone deletes a message")]
        public async Task showDelete([Summary("true/false")] bool enabled)
        {
            var modChannelId = (ulong)_redis.HashGet($"{Context.Guild.Id}:Settings", "ModChannel");
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
    }
}