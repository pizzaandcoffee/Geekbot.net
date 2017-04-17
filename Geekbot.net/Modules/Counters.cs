using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    public class Counters : ModuleBase
    {
        [Command("good"), Summary("Increase Someones Karma")]
        public async Task Good([Summary("The someone")] IUser user)
        {
            if (user.Id == Context.User.Id)
            {
                await ReplyAsync($"Sorry {Context.User.Username}, but you can't give yourself karma");
            }
            else
            {
                var redis = new RedisClient().Client;
                var key = Context.Guild.Id + "-" + user.Id + "-karma";
                var badJokes = (int)redis.StringGet(key);
                redis.StringSet(key, (badJokes + 1).ToString());
                await ReplyAsync($"{Context.User.Username} gave {user.Mention} karma");
            }
        }

        [Command("bad"), Summary("Decrease Someones Karma")]
        public async Task Bad([Summary("The someone")] IUser user)
        {
            if (user.Id == Context.User.Id)
            {
                await ReplyAsync($"Sorry {Context.User.Username}, but you can't lower your own karma");
            }
            else
            {
                var redis = new RedisClient().Client;
                var key = Context.Guild.Id + "-" + user.Id + "-karma";
                var badJokes = (int)redis.StringGet(key);
                redis.StringSet(key, (badJokes - 1).ToString());
                await ReplyAsync($"{Context.User.Username} lowered {user.Mention}'s karma");
            }
        }
    }
}