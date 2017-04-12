using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Geekbot.net.Modules
{
    public class UserInfo : ModuleBase
    {
        [Alias("stats", "whois")]
        [Command("user"), Summary("Get information about this user")]
        public async Task User([Summary("The (optional) user to get info for")] IUser user = null)
        {
            var userInfo = user ?? Context.Message.Author;

            var age = Math.Floor((DateTime.Now - userInfo.CreatedAt).TotalDays);

            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}\r\n" +
                             $"Account created at {userInfo.CreatedAt.Day}.{userInfo.CreatedAt.Month}.{userInfo.CreatedAt.Year}, that is {age} days ago\r\n" +
                             $"Currently {userInfo.Status}");
        }
    }
}