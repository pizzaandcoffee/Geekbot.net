using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Modules
{
    public class UserInfo : ModuleBase
    {
        private readonly IDatabase redis;

        public UserInfo(IDatabase redis)
        {
            this.redis = redis;
        }

        [Command("stats", RunMode = RunMode.Async)]
        [Summary("Get information about this user")]
        public async Task User([Summary("@someone")] IUser user = null)
        {
            var userInfo = user ?? Context.Message.Author;

            var age = Math.Floor((DateTime.Now - userInfo.CreatedAt).TotalDays);

            var key = Context.Guild.Id + "-" + userInfo.Id;
            var messages = (int) redis.StringGet(key + "-messages");
            var level = LevelCalc.GetLevelAtExperience(messages);

            var guildKey = Context.Guild.Id.ToString();
            var guildMessages = (int) redis.StringGet(guildKey + "-messages");

            var percent = Math.Round((double) (100 * messages) / guildMessages, 2);

            var eb = new EmbedBuilder();
            eb.WithAuthor(new EmbedAuthorBuilder()
                .WithIconUrl(userInfo.GetAvatarUrl())
                .WithName(userInfo.Username));

            eb.WithColor(new Color(221, 255, 119));

            eb.AddField("Discordian Since",
                $"{userInfo.CreatedAt.Day}/{userInfo.CreatedAt.Month}/{userInfo.CreatedAt.Year} ({age} days)");
            eb.AddInlineField("Level", level)
                .AddInlineField("Messages Sent", messages)
                .AddInlineField("Server Total", $"{percent}%");

            var karma = redis.StringGet(key + "-karma");
            if (!karma.IsNullOrEmpty)
                eb.AddInlineField("Karma", karma);

            var correctRolls = redis.StringGet($"{Context.Guild.Id}-{userInfo.Id}-correctRolls");
            if (!correctRolls.IsNullOrEmpty)
                eb.AddInlineField("Guessed Rolls", correctRolls);

            await ReplyAsync("", false, eb.Build());
        }

        [Command("rank", RunMode = RunMode.Async)]
        [Summary("get user top 10")]
        public async Task Rank()
        {
            await ReplyAsync("this will take a moment...");
            var guildKey = Context.Guild.Id.ToString();
            var guildMessages = (int) redis.StringGet(guildKey + "-messages");
            var allGuildUsers = await Context.Guild.GetUsersAsync();
            var unsortedDict = new Dictionary<string, int>();
            foreach (var user in allGuildUsers)
            {
                var key = Context.Guild.Id + "-" + user.Id;
                var messages = (int) redis.StringGet(key + "-messages");
                if (messages > 0)
                    unsortedDict.Add($"{user.Username}#{user.Discriminator}", messages);
            }
            var sortedDict = unsortedDict.OrderByDescending(x => x.Value);
            var reply = new StringBuilder();
            reply.AppendLine($"Total Messages on {Context.Guild.Name}: {guildMessages}");
            var count = 1;
            foreach (var entry in sortedDict)
                if (count < 11)
                {
                    var percent = Math.Round((double) (100 * entry.Value) / guildMessages, 2);
                    reply.AppendLine($"#{count} - **{entry.Key}** - {percent}% of total - {entry.Value} messages");
                    count++;
                }
                else
                {
                    break;
                }
            await ReplyAsync(reply.ToString());
        }
    }
}