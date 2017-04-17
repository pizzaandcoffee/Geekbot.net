using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace Geekbot.net.Modules
{
    public class Info : ModuleBase
    {
        [Command("info"), Summary("Show some info about the bot.")]
        public async Task getInfo()
        {
            var eb = new EmbedBuilder();
            eb.WithAuthor(new EmbedAuthorBuilder()
                .WithIconUrl(Context.User.GetAvatarUrl())
                .WithName(Context.User.Username));
            eb.AddField("Test", "Some testing stuff here...");
            await ReplyAsync("", false, eb.Build());
        }
    }
}