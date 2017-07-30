using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.IClients;

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
        [Command("user", RunMode = RunMode.Async), Summary("Get information about this user")]
        public async Task User([Summary("The (optional) user to get info for")] IUser user = null)
        {
            var userInfo = user ?? Context.Message.Author;

            var age = Math.Floor((DateTime.Now - userInfo.CreatedAt).TotalDays);

            var key = Context.Guild.Id + "-" + userInfo.Id;
            var messages = (int)redis.Client.StringGet(key + "-messages");
            var level = LevelCalc.GetLevelAtExperience(messages);

            var guildKey = Context.Guild.Id.ToString();
            var guildMessages = (int)redis.Client.StringGet(guildKey + "-messages");

            var percent = Math.Round((double)(100 * messages) / guildMessages, 2);

            var eb = new EmbedBuilder();
            eb.WithAuthor(new EmbedAuthorBuilder()
                .WithIconUrl(userInfo.GetAvatarUrl())
                .WithName(userInfo.Username));

            eb.WithColor(new Color(221, 255, 119));

            eb.AddField("Discordian Since", $"{userInfo.CreatedAt.Day}/{userInfo.CreatedAt.Month}/{userInfo.CreatedAt.Year} ({age} days)");
            eb.AddInlineField("Level", level)
                .AddInlineField("Messages Sent", messages)
                .AddInlineField("Server Total", $"{percent}%");

            var karma = redis.Client.StringGet(key + "-karma");
            if (!karma.IsNullOrEmpty)
            {
                eb.AddInlineField("Karma", karma);
            }

            var correctRolls = redis.Client.StringGet($"{Context.Guild.Id}-{userInfo.Id}-correctRolls");
            if (!correctRolls.IsNullOrEmpty)
            {
                eb.AddInlineField("Guessed Rolls", correctRolls);
            }

            await ReplyAsync("", false, eb.Build());
        }

        [Alias("highscore")]
        [Command("rank", RunMode = RunMode.Async), Summary("get user top 10")]
        public async Task Rank() 
        {
            await ReplyAsync("this will take a moment...");
            var allGuildUsers = await Context.Guild.GetUsersAsync();
            var unsortedDict = new Dictionary<string, int>();
            foreach(var user in allGuildUsers) 
            {
                var key = Context.Guild.Id + "-" + user.Id;
                var messages = (int)redis.Client.StringGet(key + "-messages");
                if(messages > 0) {
                    unsortedDict.Add(user.Username, messages);
                }
            }
            var sortedDict = unsortedDict.OrderByDescending(x => x.Value);
            var reply = new StringBuilder();
            var count = 1;
            foreach(KeyValuePair<string, int> entry in sortedDict)
            {
                if(count < 11){
                    reply.Append($"#{count} - **{entry.Key}** - {entry.Value}\r\n");
                    count++;
                } else {
                    break;
                }
            }
            await ReplyAsync(reply.ToString());
        }
    }
}