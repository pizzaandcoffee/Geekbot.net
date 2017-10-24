using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class Stats : ModuleBase
    {
        private readonly IDatabase _redis;
        private readonly IErrorHandler _errorHandler;
        private readonly ILevelCalc _levelCalc;
        
        public Stats(IDatabase redis, IErrorHandler errorHandler, ILevelCalc levelCalc)
        {
            _redis = redis;
            _errorHandler = errorHandler;
            _levelCalc = levelCalc;
        }

        [Command("stats", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Statistics)]
        [Summary("Get information about this user")]
        public async Task User([Summary("@someone")] IUser user = null)
        {
            try
            {
                var userInfo = user ?? Context.Message.Author;
                var userGuildInfo = (IGuildUser) userInfo;
                var createdAt = userInfo.CreatedAt;
                var joinedAt = userGuildInfo.JoinedAt.Value;
                var age = Math.Floor((DateTime.Now - createdAt).TotalDays);
                var joinedDayAgo = Math.Floor((DateTime.Now - joinedAt).TotalDays);

                var messages = (int) _redis.HashGet($"{Context.Guild.Id}:Messages", userInfo.Id.ToString());
                var guildMessages = (int) _redis.HashGet($"{Context.Guild.Id}:Messages", 0.ToString());
                var level = _levelCalc.GetLevelAtExperience(messages);

                var percent = Math.Round((double) (100 * messages) / guildMessages, 2);

                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(userInfo.GetAvatarUrl())
                    .WithName(userInfo.Username));
                eb.WithColor(new Color(221, 255, 119));
                
                var karma = _redis.HashGet($"{Context.Guild.Id}:Karma", userInfo.Id);
                var correctRolls = _redis.HashGet($"{Context.Guild.Id}:Rolls", userInfo.Id.ToString());

                eb.AddInlineField("Discordian Since", $"{createdAt.Day}.{createdAt.Month}.{createdAt.Year} ({age} days)")
                    .AddInlineField("Joined Server", $"{joinedAt.Day}.{joinedAt.Month}.{joinedAt.Year} ({joinedDayAgo} days)")
                    .AddInlineField("Karma", karma.ToString() ?? "0")
                    .AddInlineField("Level", level)
                    .AddInlineField("Messages Sent", messages)
                    .AddInlineField("Server Total", $"{percent}%");

                if (!correctRolls.IsNullOrEmpty)
                    eb.AddInlineField("Guessed Rolls", correctRolls);

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}