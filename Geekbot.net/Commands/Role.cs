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
        private readonly IReactionListener _reactionListener;

        public Role(IErrorHandler errorHandler, IDatabase redis, IReactionListener reactionListener)
        {
            _errorHandler = errorHandler;
            _redis = redis;
            _reactionListener = reactionListener;
        }

        [Command(RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Get a list of all available roles.")]
        public async Task GetAllRoles()
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
        public async Task GiveRole([Summary("roleNickname")] string roleNameRaw)
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

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("add", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Add a role to the whitelist.")]
        public async Task AddRole([Summary("@role")] IRole role, [Summary("alias")] string roleName)
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

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("remove", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Remove a role from the whitelist.")]
        public async Task RemoveRole([Summary("roleNickname")] string roleName)
        {
            try
            {
                
                var success = _redis.HashDelete($"{Context.Guild.Id}:RoleWhitelist", roleName.ToLower());
                if (success)
                {
                    await ReplyAsync($"Removed {roleName} from the whitelist");
                    return;
                }

                await ReplyAsync("There is not whitelisted role with that name...");

            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Remarks(CommandCategories.Admin)]
        [Summary("Give a role by clicking on an emoji")]
        [Command("listen", RunMode = RunMode.Async)]
        public async Task AddListener([Summary("messageID")] string messageId, [Summary("Emoji")] string emoji, [Summary("@role")] IRole role)
        {
            try
            {
                var message = (IUserMessage) await Context.Channel.GetMessageAsync(ulong.Parse(messageId));
                IEmote emote;
                if (!emoji.StartsWith('<'))
                {
                    var emo = new Emoji(emoji);
                    emote = emo;
                }
                else
                {
                    emote = Emote.Parse(emoji);
                }
                await message.AddReactionAsync(emote);
                await _reactionListener.AddRoleToListener(messageId, emote, role);
                await Context.Message.DeleteAsync();
            }
            catch (HttpException e)
            {
                await Context.Channel.SendMessageAsync("Custom emojis from other servers are not supported");
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                await Context.Channel.SendMessageAsync("Something went wrong... please try again on a new message");
                Console.WriteLine(e);
            }
        }
    }
}