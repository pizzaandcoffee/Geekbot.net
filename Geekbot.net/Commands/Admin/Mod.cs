using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.UserRepository;

namespace Geekbot.net.Commands.Admin
{
    [Group("mod")]
    [RequireUserPermission(GuildPermission.KickMembers)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public class Mod : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly IErrorHandler _errorHandler;
        private readonly IUserRepository _userRepository;

        public Mod(IUserRepository userRepositry, IErrorHandler errorHandler, DiscordSocketClient client)
        {
            _userRepository = userRepositry;
            _errorHandler = errorHandler;
            _client = client;
        }

        [Command("namehistory", RunMode = RunMode.Async)]
        [Summary("See past usernames of an user")]
        public async Task UsernameHistory([Summary("@user")] IUser user)
        {
            try
            {
                var userRepo = _userRepository.Get(user.Id);
                if (userRepo != null && userRepo.UsedNames != null)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($":bust_in_silhouette: {user.Username} has been known as:");
                    foreach (var name in userRepo.UsedNames) sb.AppendLine($"- `{name.Name}`");
                    await ReplyAsync(sb.ToString());
                }
                else
                {
                    await ReplyAsync($"No name changes found for {user.Username}");
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context,
                    $"I don't have enough permissions do that");
            }
        }
    }
}