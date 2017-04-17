using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

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

            var redis = new RedisClient().Client;
            var key = Context.Guild.Id + "-" + userInfo.Id + "-messages";
            var messages = (int)redis.StringGet(key);
            var level = GetLevelAtExperience(messages);

            var reply = "";

            if (Context.Message.Author.Id == userInfo.Id)
            {
                reply = reply + $"here are your stats {userInfo.Mention}\r\n";
            }
            else
            {
                reply = reply + $"here are {userInfo.Mention}'s stats\r\n";
            }

            reply = reply + $"```\r\n";
            reply = reply + $"Level:            {level}\r\n";
            reply = reply + $"Messages Sent:    {messages}\r\n";
            reply =
                reply +
                $"Discordian Since: {userInfo.CreatedAt.Day}/{userInfo.CreatedAt.Month}/{userInfo.CreatedAt.Year} ({age} days)";
            reply = reply + $"```";

            await ReplyAsync(reply);
        }

        [Command("level"), Summary("Get a level based on a number")]
        public async Task GetLevel([Summary("The (optional) user to get info for")] string xp)
        {
            var level = GetLevelAtExperience(int.Parse(xp));
            await ReplyAsync(level.ToString());
        }

        public int GetExperienceAtLevel(int level){
            double total = 0;
            for (int i = 1; i < level; i++)
            {
                total += Math.Floor(i + 300 * Math.Pow(2, i / 7.0));
            }

            return (int) Math.Floor(total / 16);
        }

        public int GetLevelAtExperience(int experience) {
            int index;

            for (index = 0; index < 120; index++) {
                if (GetExperienceAtLevel(index + 1) > experience)
                    break;
            }

            return index;
        }
    }
}