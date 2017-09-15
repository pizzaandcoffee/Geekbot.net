using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using StackExchange.Redis;

namespace Geekbot.net.Modules
{
    public class Counters : ModuleBase
    {
        private readonly IDatabase redis;

        public Counters(IDatabase redis)
        {
            this.redis = redis;
        }

        [Command("good", RunMode = RunMode.Async)]
        [Summary("Increase Someones Karma")]
        public async Task Good([Summary("@someone")] IUser user)
        {
            var lastKarma = GetLastKarma();
            if (user.Id == Context.User.Id)
            {
                await ReplyAsync($"Sorry {Context.User.Username}, but you can't give yourself karma");
            }
            else if (lastKarma > GetUnixTimestamp())
            {
                await ReplyAsync(
                    $"Sorry {Context.User.Username}, but you have to wait {GetTimeLeft(lastKarma)} before you can give karma again...");
            }
            else
            {
                var key = Context.Guild.Id + "-" + user.Id + "-karma";
                var badJokes = (int) redis.StringGet(key);
                var newBadJokes = badJokes + 1;
                redis.StringSet(key, newBadJokes.ToString());
                var lastKey = Context.Guild.Id + "-" + Context.User.Id + "-karma-timeout";
                redis.StringSet(lastKey, GetNewLastKarma());

                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(user.GetAvatarUrl())
                    .WithName(user.Username));

                eb.WithColor(new Color(138, 219, 146));
                eb.Title = "Karma Increased";
                eb.AddInlineField("By", Context.User.Username);
                eb.AddInlineField("amount", "+1");
                eb.AddInlineField("Current Karma", newBadJokes);
                await ReplyAsync("", false, eb.Build());
            }
        }

        [Command("bad", RunMode = RunMode.Async)]
        [Summary("Decrease Someones Karma")]
        public async Task Bad([Summary("@someone")] IUser user)
        {
            var lastKarma = GetLastKarma();
            if (user.Id == Context.User.Id)
            {
                await ReplyAsync($"Sorry {Context.User.Username}, but you can't lower your own karma");
            }
            else if (lastKarma > GetUnixTimestamp())
            {
                await ReplyAsync(
                    $"Sorry {Context.User.Username}, but you have to wait {GetTimeLeft(lastKarma)} before you can take karma again...");
            }
            else
            {
                var key = Context.Guild.Id + "-" + user.Id + "-karma";
                var badJokes = (int) redis.StringGet(key);
                var newBadJokes = badJokes - 1;
                redis.StringSet(key, newBadJokes.ToString());
                var lastKey = Context.Guild.Id + "-" + Context.User.Id + "-karma-timeout";
                redis.StringSet(lastKey, GetNewLastKarma());

                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(user.GetAvatarUrl())
                    .WithName(user.Username));

                eb.WithColor(new Color(138, 219, 146));
                eb.Title = "Karma Decreased";
                eb.AddInlineField("By", Context.User.Username);
                eb.AddInlineField("amount", "-1");
                eb.AddInlineField("Current Karma", newBadJokes);
                await ReplyAsync("", false, eb.Build());
            }
        }

        private int GetLastKarma()
        {
            var lastKey = Context.Guild.Id + "-" + Context.User.Id + "-karma-timeout";
            var redisReturn = redis.StringGet(lastKey);
            if (!int.TryParse(redisReturn.ToString(), out var i))
                i = GetUnixTimestamp();
            return i;
        }

        private int GetNewLastKarma()
        {
            var timeout = TimeSpan.FromMinutes(3);
            return (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Add(timeout).TotalSeconds;
        }

        private int GetUnixTimestamp()
        {
            return (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private string GetTimeLeft(int time)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(time).ToLocalTime();
            var dt = dtDateTime.Subtract(DateTime.Now);
            return $"{dt.Minutes} Minutes and {dt.Seconds} Seconds";
        }
    }
}