using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Say : ModuleBase
    {
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [Command("say"), Summary("Say Something.")]
        public async Task Echo([Remainder, Summary("What?")] string echo)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(echo);
        }
    }
}