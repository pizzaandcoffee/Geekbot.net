﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    public class UserInfo : ModuleBase
    {
        [Alias("stats")]
        [Command("user"), Summary("Get information about this user")]
        public async Task User([Summary("The (optional) user to get info for")] IUser user = null)
        {
            var userInfo = user ?? Context.Message.Author;

            var age = Math.Floor((DateTime.Now - userInfo.CreatedAt).TotalDays);

            var redis = new RedisClient().Client;
            var key = Context.Guild.Id + "-" + userInfo.Id;
            var messages = (int)redis.StringGet(key + "-messages");
            var level = GetLevelAtExperience(messages);

            var eb = new EmbedBuilder();
            eb.WithAuthor(new EmbedAuthorBuilder()
                .WithIconUrl(userInfo.GetAvatarUrl())
                .WithName(userInfo.Username));
            
            eb.AddField("Discordian Since", $"{userInfo.CreatedAt.Day}/{userInfo.CreatedAt.Month}/{userInfo.CreatedAt.Year} ({age} days)");
            eb.AddField("Level", level);
            eb.AddField("Messages Sent", messages);

            var karma = redis.StringGet(key + "-karma");
            if (!karma.IsNullOrEmpty)
            {
                eb.AddField("Karma", karma);
            }
                                    
            await ReplyAsync("", false, eb.Build());
        }

        public async Task GetLevel(string xp)
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