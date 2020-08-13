using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.Database;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.Levels;

namespace Geekbot.Bot.Commands.User
{
    public class Stats : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly ILevelCalc _levelCalc;
        private readonly DatabaseContext _database;

        public Stats(DatabaseContext database, IErrorHandler errorHandler, ILevelCalc levelCalc)
        {
            _database = database;
            _errorHandler = errorHandler;
            _levelCalc = levelCalc;
        }

        [Command("stats", RunMode = RunMode.Async)]
        [Summary("Get information about this user")]
        [DisableInDirectMessage]
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
                        ?.FirstOrDefault(e => e.GuildId.Equals(Context.Guild.Id.AsLong()) && e.UserId.Equals(userInfo.Id.AsLong()))
                        ?.MessageCount ?? 0;
                var guildMessages = _database.Messages
                        ?.Where(e => e.GuildId.Equals(Context.Guild.Id.AsLong()))
                        .Select(e => e.MessageCount)
                        .Sum() ?? 0;

                var level = _levelCalc.GetLevel(messages);

                var percent = Math.Round((double) (100 * messages) / guildMessages, 2);

                var cookies = _database.Cookies
                      ?.FirstOrDefault(e => e.GuildId.Equals(Context.Guild.Id.AsLong()) && e.UserId.Equals(userInfo.Id.AsLong()))
                      ?.Cookies ?? 0;

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
                if (cookies > 0) eb.AddInlineField("Cookies", cookies);

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}