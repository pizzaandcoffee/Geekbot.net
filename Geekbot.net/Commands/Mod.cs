using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    [Group("mod")]
    [RequireUserPermission(GuildPermission.KickMembers)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    public class Mod : ModuleBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IErrorHandler _errorHandler;
        
        public Mod(IUserRepository userRepositry, IErrorHandler errorHandler)
        {
            _userRepository = userRepositry;
            _errorHandler = errorHandler;
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
                foreach (var name in userRepo.UsedNames)
                {
                    sb.AppendLine($"- `{name}`");
                }
                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context, "Modchannel doesn't seem to exist, please set one with `!admin modchannel [channelId]`");
            }
        }
    }
}