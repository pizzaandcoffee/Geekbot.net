using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class Google : ModuleBase
    {
        [Command("google", RunMode = RunMode.Async)]
        [Summary("Google Something.")]
        public async Task Eyes([Remainder, Summary("SearchText")] string searchText)
        {
            var url = $"http://lmgtfy.com/?q={searchText.Replace(' ', '+')}";
            
            await ReplyAsync($"Please click here :unamused:\r\n{url}");
        }
    }
}