using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class GuildInfo : ModuleBase
    {
        private readonly IDatabase _redis;
        private readonly ILevelCalc _levelCalc;
        private readonly IErrorHandler _errorHandler;

        public GuildInfo(IDatabase redis, ILevelCalc levelCalc, IErrorHandler errorHandler)
        {
            _redis = redis;
            _levelCalc = levelCalc;
            _errorHandler = errorHandler;
        }

        [Command("serverstats", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Statistics)]
        [Summary("Show some info about the bot.")]
        public async Task getInfo()
        {
            try
            {
                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(Context.Guild.IconUrl)
                    .WithName(Context.Guild.Name));
                eb.WithColor(new Color(110, 204, 147));

                var created = Context.Guild.CreatedAt;
                var age = Math.Floor((DateTime.Now - created).TotalDays);

                var messages = _redis.HashGet($"{Context.Guild.Id}:Messages", 0.ToString());
                var level = _levelCalc.GetLevelAtExperience((int) messages);

                eb.AddField("Server Age", $"{created.Day}/{created.Month}/{created.Year} ({age} days)");
                eb.AddInlineField("Level", level)
                    .AddInlineField("Messages", messages);

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}