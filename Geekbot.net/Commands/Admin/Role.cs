using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.ReactionListener;

namespace Geekbot.net.Commands.Admin
{
    [Group("role")]
    public class Role : ModuleBase
    {
        private readonly DatabaseContext _database;
        private readonly IErrorHandler _errorHandler;
        private readonly IReactionListener _reactionListener;

        public Role(DatabaseContext database, IErrorHandler errorHandler, IReactionListener reactionListener)
        {
            _database = database;
            _errorHandler = errorHandler;
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
                    await ReplyAsync("There are no roles configured for this server");
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine($"**Self Service Roles on {Context.Guild.Name}**");
                sb.AppendLine("To get a role, use `!role name`");
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
        public async Task GiveRole([Summary("roleNickname")] string roleNameRaw)
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
                        await ReplyAsync("That role doesn't seem to exist");
                        return;
                    }

                    if (guildUser.RoleIds.Contains(role.Id))
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
                        "You cannot add that role to self service because it contains one or more dangerous permissions");
                    return;
                }

                _database.RoleSelfService.Add(new RoleSelfServiceModel
                {
                    GuildId = Context.Guild.Id.AsLong(),
                    RoleId = role.Id.AsLong(),
                    WhiteListName = roleName
                });
                await _database.SaveChangesAsync();
                await ReplyAsync($"Added {role.Name} to the whitelist");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("remove", RunMode = RunMode.Async)]
        [Summary("Remove a role from the whitelist.")]
        public async Task RemoveRole([Summary("roleNickname")] string roleName)
        {
            try
            {
                var roleFromDb = _database.RoleSelfService.FirstOrDefault(e =>
                    e.GuildId.Equals(Context.Guild.Id.AsLong()) && e.WhiteListName.Equals(roleName));
                if (roleFromDb != null)
                {
                    _database.RoleSelfService.Remove(roleFromDb);
                    await _database.SaveChangesAsync();
                    await ReplyAsync($"Removed {roleName} from the whitelist");
                    return;
                }

                await ReplyAsync("There is not whitelisted role with that name");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [RequireUserPermission(GuildPermission.ManageRoles)]
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