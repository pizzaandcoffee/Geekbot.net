using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
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
    public class Role : ModuleBase
    {
        private readonly DatabaseContext _database;
        private readonly IErrorHandler _errorHandler;
        private readonly IReactionListener _reactionListener;
        private readonly ITranslationHandler _translationHandler;

        public Role(DatabaseContext database, IErrorHandler errorHandler, IReactionListener reactionListener, ITranslationHandler translationHandler)
        {
            _database = database;
            _errorHandler = errorHandler;
            _reactionListener = reactionListener;
            _translationHandler = translationHandler;
        }

        [Command(RunMode = RunMode.Async)]
        [Summary("Get a list of all available roles.")]
        public async Task GetAllRoles()
        {
            try
            {
                var transContext = await _translationHandler.GetGuildContext(Context);
                var roles = _database.RoleSelfService.Where(g => g.GuildId.Equals(Context.Guild.Id.AsLong())).ToList();
                if (roles.Count == 0)
                {
                    await ReplyAsync(transContext.GetString("NoRolesConfigured"));
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine(transContext.GetString("ListHeader", Context.Guild.Name));
                sb.AppendLine(transContext.GetString("ListInstruction"));
                foreach (var role in roles) sb.AppendLine($"- {role.WhiteListName}");
                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command(RunMode = RunMode.Async)]
        [Summary("Get a role by mentioning it.")]
        public async Task GiveRole([Summary("role-nickname")] string roleNameRaw)
        {
            try
            {
                var transContext = await _translationHandler.GetGuildContext(Context);
                var roleName = roleNameRaw.ToLower();
                var roleFromDb = _database.RoleSelfService.FirstOrDefault(e =>
                    e.GuildId.Equals(Context.Guild.Id.AsLong()) && e.WhiteListName.Equals(roleName));
                if (roleFromDb != null)
                {
                    var guildUser = (IGuildUser) Context.User;
                    var role = Context.Guild.Roles.First(r => r.Id == roleFromDb.RoleId.AsUlong());
                    if (role == null)
                    {
                        await ReplyAsync(transContext.GetString("RoleNotFound"));
                        return;
                    }

                    if (guildUser.RoleIds.Contains(role.Id))
                    {
                        await guildUser.RemoveRoleAsync(role);
                        await ReplyAsync(transContext.GetString("RemovedUserFromRole", role.Name));
                        return;
                    }

                    await guildUser.AddRoleAsync(role);
                    await ReplyAsync(transContext.GetString("AddedUserFromRole", role.Name));
                    return;
                }

                await ReplyAsync(transContext.GetString("RoleNotFound"));
            }
            catch (HttpException e)
            {
                await _errorHandler.HandleHttpException(e, Context);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("add", RunMode = RunMode.Async)]
        [Summary("Add a role to the whitelist.")]
        public async Task AddRole([Summary("@role")] IRole role, [Summary("alias")] string roleName)
        {
            try
            {
                var transContext = await _translationHandler.GetGuildContext(Context);
                if (role.IsManaged)
                {
                    await ReplyAsync(transContext.GetString("CannotAddManagedRole"));
                    return;
                }

                if (role.Permissions.ManageRoles
                    || role.Permissions.Administrator
                    || role.Permissions.ManageGuild
                    || role.Permissions.BanMembers
                    || role.Permissions.KickMembers)
                {
                    await ReplyAsync(transContext.GetString("CannotAddDangerousRole"));
                    return;
                }

                _database.RoleSelfService.Add(new RoleSelfServiceModel
                {
                    GuildId = Context.Guild.Id.AsLong(),
                    RoleId = role.Id.AsLong(),
                    WhiteListName = roleName
                });
                await _database.SaveChangesAsync();
                await ReplyAsync(transContext.GetString("AddedRoleToWhitelist", role.Name));
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("remove", RunMode = RunMode.Async)]
        [Summary("Remove a role from the whitelist.")]
        public async Task RemoveRole([Summary("role-nickname")] string roleName)
        {
            try
            {
                var transContext = await _translationHandler.GetGuildContext(Context);
                var roleFromDb = _database.RoleSelfService.FirstOrDefault(e =>
                    e.GuildId.Equals(Context.Guild.Id.AsLong()) && e.WhiteListName.Equals(roleName));
                if (roleFromDb != null)
                {
                    _database.RoleSelfService.Remove(roleFromDb);
                    await _database.SaveChangesAsync();
                    await ReplyAsync(transContext.GetString("RemovedRoleFromWhitelist", roleName));
                    return;
                }

                await ReplyAsync(transContext.GetString("RoleNotFound"));
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
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
            catch (HttpException e)
            {
                await Context.Channel.SendMessageAsync("Custom emojis from other servers are not supported");
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}