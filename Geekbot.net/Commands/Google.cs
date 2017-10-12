using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Google : ModuleBase
    {
        [Command("google", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Fun)]
        [Summary("Google Something.")]
        public async Task Eyes([Remainder, Summary("SearchText")] string searchText)
        {
            var url = $"http://lmgtfy.com/?q={searchText.Replace(' ', '+')}";
            
            await ReplyAsync($"Please click here :unamused:\r\n{url}");
        }
    }
}