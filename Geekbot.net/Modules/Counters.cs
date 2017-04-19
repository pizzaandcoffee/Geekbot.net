using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    public class Counters : ModuleBase
    {
        private readonly IRedisClient redis;
        public Counters(IRedisClient redisClient)
        {
            redis = redisClient;
        }

        [Command("good"), Summary("Increase Someones Karma")]
        public async Task Good([Summary("The someone")] IUser user)
        {
            var lastKarma = GetLastKarma();
            Console.WriteLine(lastKarma.ToString());
            if (user.Id == Context.User.Id)
            {
                await ReplyAsync($"Sorry {Context.User.Username}, but you can't give yourself karma");
            }
            else if (lastKarma > GetUnixTimestamp())
            {
                await ReplyAsync($"Sorry {Context.User.Username}, but you have to wait {GetTimeLeft(lastKarma)} before you can give karma again...");
            }
            else
            {
                var key = Context.Guild.Id + "-" + user.Id + "-karma";
                var badJokes = (int)redis.Client.StringGet(key);
                redis.Client.StringSet(key, (badJokes + 1).ToString());
                var lastKey = Context.Guild.Id + "-" + Context.User.Id + "-karma-timeout";
                redis.Client.StringSet(lastKey, GetNewLastKarma());
                await ReplyAsync($"{Context.User.Username} gave {user.Mention} karma");
            }
        }

        [Command("bad"), Summary("Decrease Someones Karma")]
        public async Task Bad([Summary("The someone")] IUser user)
        {
            var lastKarma = GetLastKarma();
            if (user.Id == Context.User.Id)
            {
                await ReplyAsync($"Sorry {Context.User.Username}, but you can't lower your own karma");
            }
            else if (lastKarma > GetUnixTimestamp())
            {
                await ReplyAsync($"Sorry {Context.User.Username}, but you have to wait {GetTimeLeft(lastKarma)} before you can take karma again...");
            }
            else
            {
                var key = Context.Guild.Id + "-" + user.Id + "-karma";
                var badJokes = (int)redis.Client.StringGet(key);
                redis.Client.StringSet(key, (badJokes - 1).ToString());
                var lastKey = Context.Guild.Id + "-" + Context.User.Id + "-karma-timeout";
                redis.Client.StringSet(lastKey, GetNewLastKarma());
                await ReplyAsync($"{Context.User.Username} lowered {user.Mention}'s karma");
            }
        }

        private int GetLastKarma()
        {
            var lastKey = Context.Guild.Id + "-" + Context.User.Id + "-karma-timeout";
            var redisReturn = redis.Client.StringGet(lastKey);
            if (!int.TryParse(redisReturn.ToString(), out var i))
            {
                i = GetUnixTimestamp();
            }
            return i;
        }

        private int GetNewLastKarma()
        {
            var timeout = TimeSpan.FromMinutes(3);
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).Add(timeout)).TotalSeconds;
        }

        private int GetUnixTimestamp()
        {
            return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        private string GetTimeLeft(int time)
        {
            DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds( time ).ToLocalTime();
            var dt = dtDateTime.Subtract(DateTime.Now);
            return $"{dt.Minutes} Minutes and {dt.Seconds} Seconds";
        }
    }
}