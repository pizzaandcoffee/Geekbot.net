using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Admin
{
    [Group("mod")]
    [RequireUserPermission(GuildPermission.KickMembers)]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [RequireUserPermission(GuildPermission.ManageRoles)]
    [DisableInDirectMessage]
    public class Mod : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Mod(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("namehistory", RunMode = RunMode.Async)]
        [Summary("See past usernames of an user")]
        public async Task UsernameHistory([Summary("@someone")] IUser user)
        {
            try
            {
                await Context.Channel.SendMessageAsync("This command has been removed due to low usage and excessively high database usage");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}