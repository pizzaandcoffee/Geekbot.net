using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Geekbot.net.Commands
{
    public class Say : ModuleBase
    {
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("say", RunMode = RunMode.Async)]
        [Summary("Say Something.")]
        public async Task Echo([Remainder] [Summary("What?")] string echo)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(echo);
        }
    }
}