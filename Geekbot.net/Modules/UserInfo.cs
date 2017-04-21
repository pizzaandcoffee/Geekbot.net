using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    public class UserInfo : ModuleBase
    {
        private readonly IRedisClient redis;
        public UserInfo(IRedisClient redisClient)
        {
            redis = redisClient;
        }

        [Alias("stats")]
        [Command("user"), Summary("Get information about this user")]
        public async Task User([Summary("The (optional) user to get info for")] IUser user = null)
        {
            var userInfo = user ?? Context.Message.Author;

            var age = Math.Floor((DateTime.Now - userInfo.CreatedAt).TotalDays);

            var key = Context.Guild.Id + "-" + userInfo.Id;
            var messages = (int)redis.Client.StringGet(key + "-messages");
            var level = LevelCalc.GetLevelAtExperience(messages);

            var eb = new EmbedBuilder();
            eb.WithAuthor(new EmbedAuthorBuilder()
                .WithIconUrl(userInfo.GetAvatarUrl())
                .WithName(userInfo.Username));
            eb.WithColor(new Color(221, 255, 119));

            eb.AddField("Discordian Since", $"{userInfo.CreatedAt.Day}/{userInfo.CreatedAt.Month}/{userInfo.CreatedAt.Year} ({age} days)");
            eb.AddInlineField("Level", level)
                .AddInlineField("Messages Sent", messages);

            var karma = redis.Client.StringGet(key + "-karma");
            if (!karma.IsNullOrEmpty)
            {
                eb.AddField("Karma", karma);
            }
                                    
            await ReplyAsync("", false, eb.Build());
        }

        
    }
}