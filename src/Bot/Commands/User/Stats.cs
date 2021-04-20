using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.Database;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.Levels;

namespace Geekbot.Bot.Commands.User
{
    public class Stats : GeekbotCommandBase
    {
        private readonly ILevelCalc _levelCalc;
        private readonly DatabaseContext _database;

        public Stats(DatabaseContext database, IErrorHandler errorHandler, ILevelCalc levelCalc, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
            _database = database;
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

                var quotes = _database.Quotes.Count(e => e.GuildId.Equals(Context.Guild.Id.AsLong()) && e.UserId.Equals(userInfo.Id.AsLong()));

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

                eb.AddInlineField(Localization.Stats.OnDiscordSince,
                        $"{createdAt.Day}.{createdAt.Month}.{createdAt.Year} ({age} {Localization.Stats.Days})")
                    .AddInlineField(Localization.Stats.JoinedServer,
                        $"{joinedAt.Day}.{joinedAt.Month}.{joinedAt.Year} ({joinedDayAgo} {Localization.Stats.Days})")
                    .AddInlineField(Localization.Stats.Karma, karma?.Karma ?? 0)
                    .AddInlineField(Localization.Stats.Level, level)
                    .AddInlineField(Localization.Stats.MessagesSent, messages)
                    .AddInlineField(Localization.Stats.ServerTotal, $"{percent}%");

                if (correctRolls != null) eb.AddInlineField(Localization.Stats.GuessedRolls, correctRolls.Rolls);
                if (cookies > 0) eb.AddInlineField(Localization.Stats.Cookies, cookies);
                if (quotes > 0) eb.AddInlineField(Localization.Stats.Quotes, quotes);

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}