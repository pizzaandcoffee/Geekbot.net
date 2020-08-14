using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Geekbot.Core;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.Localization;
using Geekbot.Core.ReactionListener;

namespace Geekbot.Bot.Commands.Admin
{
    [Group("role")]
    [DisableInDirectMessage]
    public class Role : GeekbotCommandBase
    {
        private readonly DatabaseContext _database;
        private readonly IReactionListener _reactionListener;

        public Role(DatabaseContext database, IErrorHandler errorHandler, IReactionListener reactionListener, ITranslationHandler translationHandler) : base(errorHandler, translationHandler)
        {
            _database = database;
            _reactionListener = reactionListener;
        }

        [Command(RunMode = RunMode.Async)]
        [Summary("Get a list of all available roles.")]
        public async Task GetAllRoles()
        {
            try
            {
                var roles = _database.RoleSelfService.Where(g => g.GuildId.Equals(Context.Guild.Id.AsLong())).ToList();
                if (roles.Count == 0)
                {
                    await ReplyAsync(Localization.Role.NoRolesConfigured);
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine(string.Format(Localization.Role.ListHeader, Context.Guild.Name));
                sb.AppendLine(Localization.Role.ListInstruction);
                foreach (var role in roles) sb.AppendLine($"- {role.WhiteListName}");
                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [Command(RunMode = RunMode.Async)]
        [Summary("Get a role by mentioning it.")]
        public async Task GiveRole([Summary("role-nickname")] string roleNameRaw)
        {
            try
            {
                var roleName = roleNameRaw.ToLower();
                var roleFromDb = _database.RoleSelfService.FirstOrDefault(e =>
                    e.GuildId.Equals(Context.Guild.Id.AsLong()) && e.WhiteListName.Equals(roleName));
                if (roleFromDb != null)
                {
                    var guildUser = (IGuildUser) Context.User;
                    var role = Context.Guild.Roles.First(r => r.Id == roleFromDb.RoleId.AsUlong());
                    if (role == null)
                    {
                        await ReplyAsync(Localization.Role.RoleNotFound);
                        return;
                    }

                    if (guildUser.RoleIds.Contains(role.Id))
                    {
                        await guildUser.RemoveRoleAsync(role);
                        await ReplyAsync(string.Format(Localization.Role.RemovedUserFromRole, role.Name));
                        return;
                    }

                    await guildUser.AddRoleAsync(role);
                    await ReplyAsync(string.Format(Localization.Role.AddedUserFromRole, role.Name));
                    return;
                }

                await ReplyAsync(Localization.Role.RoleNotFound);
            }
            catch (HttpException e)
            {
                await ErrorHandler.HandleHttpException(e, Context);
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("add", RunMode = RunMode.Async)]
        [Summary("Add a role to the whitelist.")]
        public async Task AddRole([Summary("@role")] IRole role, [Summary("alias")] string roleName)
        {
            try
            {
                if (role.IsManaged)
                {
                    await ReplyAsync(Localization.Role.CannotAddManagedRole);
                    return;
                }

                if (role.Permissions.ManageRoles
                    || role.Permissions.Administrator
                    || role.Permissions.ManageGuild
                    || role.Permissions.BanMembers
                    || role.Permissions.KickMembers)
                {
                    await ReplyAsync(Localization.Role.CannotAddDangerousRole);
                    return;
                }

                _database.RoleSelfService.Add(new RoleSelfServiceModel
                {
                    GuildId = Context.Guild.Id.AsLong(),
                    RoleId = role.Id.AsLong(),
                    WhiteListName = roleName
                });
                await _database.SaveChangesAsync();
                await ReplyAsync(string.Format(Localization.Role.AddedRoleToWhitelist, role.Name));
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("remove", RunMode = RunMode.Async)]
        [Summary("Remove a role from the whitelist.")]
        public async Task RemoveRole([Summary("role-nickname")] string roleName)
        {
            try
            {
                var roleFromDb = _database.RoleSelfService.FirstOrDefault(e =>
                    e.GuildId.Equals(Context.Guild.Id.AsLong()) && e.WhiteListName.Equals(roleName));
                if (roleFromDb != null)
                {
                    _database.RoleSelfService.Remove(roleFromDb);
                    await _database.SaveChangesAsync();
                    await ReplyAsync(string.Format(Localization.Role.RemovedRoleFromWhitelist, roleName));
                    return;
                }

                await ReplyAsync(Localization.Role.RoleNotFound);
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
        
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Summary("Give a role by clicking on an emoji")]
        [Command("listen", RunMode = RunMode.Async)]
        public async Task AddListener([Summary("message-ID")] string messageIdStr, [Summary("Emoji")] string emoji, [Summary("@role")] IRole role)
        {
            try
            {
                var messageId = ulong.Parse(messageIdStr);
                var message = (IUserMessage) await Context.Channel.GetMessageAsync(messageId);
                var emote = _reactionListener.ConvertStringToEmote(emoji);
                
                await message.AddReactionAsync(emote);
                await _reactionListener.AddRoleToListener(messageId, Context.Guild.Id, emoji, role);
                await Context.Message.DeleteAsync();
            }
            catch (HttpException)
            {
                await Context.Channel.SendMessageAsync("Custom emojis from other servers are not supported");
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}