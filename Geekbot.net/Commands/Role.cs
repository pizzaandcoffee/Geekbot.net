using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    [Group("role")]
    public class Role : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;

        public Role(IErrorHandler errorHandler, IDatabase redis)
        {
            _errorHandler = errorHandler;
            _redis = redis;
        }

        [Command(RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Get a list of all available roles.")]
        public async Task getAllRoles()
        {
            try
            {
                var roles = _redis.HashGetAll($"{Context.Guild.Id}:RoleWhitelist");
                if (roles.Length == 0)
                {
                    await ReplyAsync("There are no roles configured for this server");
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine($"**Self Service Roles on {Context.Guild.Name}**");
                sb.AppendLine("To get a role, use `!role name`");
                foreach (var role in roles) sb.AppendLine($"- {role.Name}");
                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command(RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Get a role by mentioning it.")]
        public async Task giveRole([Summary("roleNickname")] string roleNameRaw)
        {
            try
            {
                var roleName = roleNameRaw.ToLower();
                if (_redis.HashExists($"{Context.Guild.Id}:RoleWhitelist", roleName))
                {
                    var guildUser = (IGuildUser) Context.User;
                    var roleId = ulong.Parse(_redis.HashGet($"{Context.Guild.Id}:RoleWhitelist", roleName));
                    var role = Context.Guild.Roles.First(r => r.Id == roleId);
                    if (role == null)
                    {
                        await ReplyAsync("That role doesn't seem to exist");
                        return;
                    }

                    if (guildUser.RoleIds.Contains(roleId))
                    {
                        await guildUser.RemoveRoleAsync(role);
                        await ReplyAsync($"Removed you from {role.Name}");
                        return;
                    }

                    await guildUser.AddRoleAsync(role);
                    await ReplyAsync($"Added you to {role.Name}");
                    return;
                }

                await ReplyAsync("That role doesn't seem to exist");
            }
            catch (HttpException e)
            {
                _errorHandler.HandleHttpException(e, Context);
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("add", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Add a role to the whitelist.")]
        public async Task addRole([Summary("@role")] IRole role, [Summary("alias")] string roleName)
        {
            try
            {
                if (role.IsManaged)
                {
                    await ReplyAsync("You can't add a role that is managed by discord");
                    return;
                }

                if (role.Permissions.ManageRoles
                    || role.Permissions.Administrator
                    || role.Permissions.ManageGuild
                    || role.Permissions.BanMembers
                    || role.Permissions.KickMembers)
                {
                    await ReplyAsync(
                        "Woah, i don't think you want to add that role to self service as it contains some dangerous permissions");
                    return;
                }

                _redis.HashSet($"{Context.Guild.Id}:RoleWhitelist",
                    new[] {new HashEntry(roleName.ToLower(), role.Id.ToString())});
                await ReplyAsync($"Added {role.Name} to the whitelist");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("remove", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Remove a role from the whitelist.")]
        public async Task removeRole([Summary("roleNickname")] string roleName)
        {
            try
            {
                _redis.HashDelete($"{Context.Guild.Id}:RoleWhitelist", roleName);
                await ReplyAsync($"Removed {roleName} from the whitelist");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}