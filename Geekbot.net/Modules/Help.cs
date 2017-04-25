using System.Threading.Tasks;
using Discord.Commands;
using System.Reflection;

namespace Geekbot.net.Modules
{
    public class Help : ModuleBase
    {
        [Command("help", RunMode = RunMode.Async), Summary("List all Commands")]
        public async Task GetHelp()
        {
            var commands = new CommandService();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            var cmdList = commands.Commands;
            var reply = "**Geekbot Command list**\r\n";
            foreach (var cmd in cmdList)
            {
                var param = string.Join(", !",cmd.Aliases);
                if (!param.Contains("admin"))
                {
                    reply = reply + $"**{cmd.Name}** (!{param}) - {cmd.Summary}\r\n";
                }
            }
            await ReplyAsync(reply);
        }
    }
}