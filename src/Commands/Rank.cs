using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Geekbot.Core.Converters;
using Geekbot.Core.Database;
using Geekbot.Core.Extensions;
using Geekbot.Core.Highscores;
using Localization = Geekbot.Core.Localization;

namespace Geekbot.Commands
{
    public class Rank
    {
        private readonly DatabaseContext _database;
        private readonly IEmojiConverter _emojiConverter;
        private readonly IHighscoreManager _highscoreManager;

        public Rank(DatabaseContext database, IEmojiConverter emojiConverter, IHighscoreManager highscoreManager)
        {
            _database = database;
            _emojiConverter = emojiConverter;
            _highscoreManager = highscoreManager;
        }

        public string Run(string typeUnformated, int amount, string season, ulong guildId, string guildName)
        {
            HighscoreTypes type;
            try
            {
                type = Enum.Parse<HighscoreTypes>(typeUnformated, true);
                if (!Enum.IsDefined(typeof(HighscoreTypes), type)) throw new Exception();
            }
            catch
            {
                return Localization.Rank.InvalidType;
            }

            var replyBuilder = new StringBuilder();
            if (amount > 20)
            {
                replyBuilder.AppendLine(Localization.Rank.LimitingTo20Warning);
                amount = 20;
            }

            Dictionary<HighscoreUserDto, int> highscoreUsers;
            try
            {
                highscoreUsers = _highscoreManager.GetHighscoresWithUserData(type, guildId, amount, season);
            }
            catch (HighscoreListEmptyException)
            {
                return string.Format(Core.Localization.Rank.NoTypeFoundForServer, type);
            }

            var guildMessages = 0;
            if (type == HighscoreTypes.messages)
            {
                guildMessages = _database.Messages
                    .Where(e => e.GuildId.Equals(guildId.AsLong()))
                    .Select(e => e.MessageCount)
                    .Sum();
            }

            var failedToRetrieveUser = highscoreUsers.Any(e => string.IsNullOrEmpty(e.Key.Username));

            if (failedToRetrieveUser) replyBuilder.AppendLine(Core.Localization.Rank.FailedToResolveAllUsernames).AppendLine();

            if (type == HighscoreTypes.seasons)
            {
                if (string.IsNullOrEmpty(season))
                {
                    season = SeasonsUtils.GetCurrentSeason();
                }

                replyBuilder.AppendLine(string.Format(Core.Localization.Rank.HighscoresFor, $"{type.ToString().CapitalizeFirst()} ({season})", guildName));
            }
            else
            {
                replyBuilder.AppendLine(string.Format(Core.Localization.Rank.HighscoresFor, type.ToString().CapitalizeFirst(), guildName));
            }

            var highscorePlace = 1;
            foreach (var (user, value) in highscoreUsers)
            {
                replyBuilder.Append(highscorePlace < 11
                    ? $"{_emojiConverter.NumberToEmoji(highscorePlace)} "
                    : $"`{highscorePlace}.` ");

                replyBuilder.Append(user.Username != null
                    ? $"**{user.Username}#{user.Discriminator}**"
                    : $"**{user.Id}**");

                replyBuilder.Append(type switch
                {
                    HighscoreTypes.messages => $" - {value} {HighscoreTypes.messages} - {Math.Round((double)(100 * value) / guildMessages, 2)}%\n",
                    HighscoreTypes.seasons => $" - {value} {HighscoreTypes.messages}\n",
                    _ => $" - {value} {type}\n"
                });

                highscorePlace++;
            }

            return replyBuilder.ToString();
        }
    }
}