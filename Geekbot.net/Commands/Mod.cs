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
    [Group("mod")]
    [RequireUserPermission(GuildPermission.KickMembers)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public class Mod : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;
        private readonly IUserRepository _userRepository;

        public Mod(IUserRepository userRepositry, IErrorHandler errorHandler, IDatabase redis,
            DiscordSocketClient client)
        {
            _userRepository = userRepositry;
            _errorHandler = errorHandler;
            _redis = redis;
            _client = client;
        }

        [Command("namehistory", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("See past usernames of an user")]
        public async Task usernameHistory([Summary("@user")] IUser user)
        {
            try
            {
                var userRepo = _userRepository.Get(user.Id);
                var sb = new StringBuilder();
                sb.AppendLine($":bust_in_silhouette: {user.Username} has been known as:");
                foreach (var name in userRepo.UsedNames) sb.AppendLine($"- `{name}`");
                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context,
                    $"I don't have enough permissions to give {user.Username} that role");
            }
        }

        [Command("kick", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Ban a user")]
        public async Task kick([Summary("@user")] IUser userNormal,
            [Summary("reason")] [Remainder] string reason = "none")
        {
            try
            {
                var user = (IGuildUser) userNormal;
                if (reason == "none") reason = "No reason provided";
                await user.GetOrCreateDMChannelAsync().Result.SendMessageAsync(
                    $"You have been kicked from {Context.Guild.Name} for the following reason: \"{reason}\"");
                await user.KickAsync();
                try
                {
                    var modChannelId = ulong.Parse(_redis.HashGet($"{Context.Guild.Id}:Settings", "ModChannel"));
                    var modChannel = (ISocketMessageChannel) _client.GetChannel(modChannelId);
                    var eb = new EmbedBuilder();
                    eb.Title = ":x: User Kicked";
                    eb.AddInlineField("User", user.Username);
                    eb.AddInlineField("By Mod", Context.User.Username);
                    eb.AddField("Reason", reason);
                    await modChannel.SendMessageAsync("", false, eb.Build());
                }
                catch
                {
                    await ReplyAsync($"{user.Username} was kicked for the following reason: \"{reason}\"");
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context, "I don't have enough permissions to kick someone");
            }
        }
    }
}