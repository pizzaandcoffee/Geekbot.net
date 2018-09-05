using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Lib.AlmostRedis;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Levels;

namespace Geekbot.net.Commands.User
{
    public class Stats : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly ILevelCalc _levelCalc;
        private readonly IAlmostRedis _redis;
        private readonly DatabaseContext _database;

        public Stats(IAlmostRedis redis, DatabaseContext database, IErrorHandler errorHandler, ILevelCalc levelCalc)
        {
            _redis = redis;
            _database = database;
            _errorHandler = errorHandler;
            _levelCalc = levelCalc;
        }

        [Command("stats", RunMode = RunMode.Async)]
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

                var messages = _database.Messages
                    .First(e => e.GuildId.Equals(Context.Guild.Id.AsLong()) && e.UserId.Equals(Context.User.Id.AsLong()))
                    .MessageCount;
                var guildMessages = _database.Messages
                    .Where(e => e.GuildId.Equals(Context.Guild.Id.AsLong()))
                    .Select(e => e.MessageCount)
                    .Sum();
//                var messages = (int) _redis.Db.HashGet($"{Context.Guild.Id}:Messages", userInfo.Id.ToString());
//                var guildMessages = (int) _redis.Db.HashGet($"{Context.Guild.Id}:Messages", 0.ToString());
                var level = _levelCalc.GetLevel(messages);

                var percent = Math.Round((double) (100 * messages) / guildMessages, 2);

                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(userInfo.GetAvatarUrl())
                    .WithName(userInfo.Username));
                eb.WithColor(new Color(221, 255, 119));

                var karma = _database.Karma.FirstOrDefault(e =>
                    e.GuildId.Equals(Context.Guild.Id.AsLong()) &&
                    e.UserId.Equals(userInfo.Id.AsLong()));
                var correctRolls = _database.Rolls.FirstOrDefault(e =>
                    e.GuildId.Equals(Context.Guild.Id.AsLong()) &&
                    e.UserId.Equals(userInfo.Id.AsLong()));

                eb.AddInlineField("Discordian Since",
                        $"{createdAt.Day}.{createdAt.Month}.{createdAt.Year} ({age} days)")
                    .AddInlineField("Joined Server",
                        $"{joinedAt.Day}.{joinedAt.Month}.{joinedAt.Year} ({joinedDayAgo} days)")
                    .AddInlineField("Karma", karma?.Karma ?? 0)
                    .AddInlineField("Level", level)
                    .AddInlineField("Messages Sent", messages)
                    .AddInlineField("Server Total", $"{percent}%");

                if (correctRolls != null) eb.AddInlineField("Guessed Rolls", correctRolls.Rolls);

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}