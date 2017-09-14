using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Geekbot.net.Lib;
using System.Linq;
using StackExchange.Redis;

namespace Geekbot.net.Modules
{
    public class GuildInfo : ModuleBase
    {
        private readonly IDatabase redis;
        public GuildInfo(IDatabase redis)
        {
            this.redis = redis;
        }

        [Command("serverstats", RunMode = RunMode.Async), Summary("Show some info about the bot.")]
        public async Task getInfo()
        {
            var eb = new EmbedBuilder();
            eb.WithAuthor(new EmbedAuthorBuilder()
               .WithIconUrl(Context.Guild.IconUrl)
               .WithName(Context.Guild.Name));
            eb.WithColor(new Color(110, 204, 147));
            
            var created = Context.Guild.CreatedAt;
            var age = Math.Floor((DateTime.Now - created).TotalDays);

            var messages = redis.StringGet($"{Context.Guild.Id}-messages");
            var level = LevelCalc.GetLevelAtExperience((int)messages);

            eb.AddField("Server Age", $"{created.Day}/{created.Month}/{created.Year} ({age} days)");
            eb.AddInlineField("Level", level)
                .AddInlineField("Messages", messages);

            await ReplyAsync("", false, eb.Build());
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}