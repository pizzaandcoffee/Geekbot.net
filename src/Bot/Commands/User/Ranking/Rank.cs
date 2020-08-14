using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.Converters;
using Geekbot.Core.Database;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.Highscores;
using Geekbot.Core.Localization;

namespace Geekbot.Bot.Commands.User.Ranking
{
    public class Rank : GeekbotCommandBase
    {
        private readonly IEmojiConverter _emojiConverter;
        private readonly IHighscoreManager _highscoreManager;
        private readonly DatabaseContext _database;

        public Rank(DatabaseContext database, IErrorHandler errorHandler, IEmojiConverter emojiConverter, IHighscoreManager highscoreManager, ITranslationHandler translations): base(errorHandler, translations)
        {
            _database = database;
            _emojiConverter = emojiConverter;
            _highscoreManager = highscoreManager;
        }

        [Command("rank", RunMode = RunMode.Async)]
        [Summary("get user top 10 in messages or karma")]
        [DisableInDirectMessage]
        public async Task RankCmd([Summary("type")] string typeUnformated = "messages", [Summary("amount")] int amount = 10)
        {
            try
            {
                HighscoreTypes type;
                try
                {
                    type = Enum.Parse<HighscoreTypes>(typeUnformated, true);
                    if (!Enum.IsDefined(typeof(HighscoreTypes), type)) throw new Exception();
                }
                catch
                {
                    await ReplyAsync(Localization.Rank.InvalidType);
                    return;
                }

                var replyBuilder = new StringBuilder();
                if (amount > 20)
                {
                    await ReplyAsync(Localization.Rank.LimitingTo20Warning);
                    amount = 20;
                }
                
                var guildId = Context.Guild.Id;
                Dictionary<HighscoreUserDto, int> highscoreUsers;
                try
                {
                    highscoreUsers = _highscoreManager.GetHighscoresWithUserData(type, guildId, amount);
                }
                catch (HighscoreListEmptyException)
                {
                    await ReplyAsync(string.Format(Localization.Rank.NoTypeFoundForServer, type));
                    return;
                }

                var guildMessages = 0;
                if (type == HighscoreTypes.messages)
                {
                    guildMessages = _database.Messages
                        .Where(e => e.GuildId.Equals(Context.Guild.Id.AsLong()))
                        .Select(e => e.MessageCount)
                        .Sum();
                }

                var failedToRetrieveUser = highscoreUsers.Any(e => string.IsNullOrEmpty(e.Key.Username));

                if (failedToRetrieveUser) replyBuilder.AppendLine(Localization.Rank.FailedToResolveAllUsernames).AppendLine();
                
                replyBuilder.AppendLine(string.Format(Localization.Rank.HighscoresFor, type.ToString().CapitalizeFirst(), Context.Guild.Name));
                
                var highscorePlace = 1;
                foreach (var (user, value) in highscoreUsers)
                {
                    replyBuilder.Append(highscorePlace < 11
                        ? $"{_emojiConverter.NumberToEmoji(highscorePlace)} "
                        : $"`{highscorePlace}.` ");

                    replyBuilder.Append(user.Username != null
                        ? $"**{user.Username}#{user.Discriminator}**"
                        : $"**{user.Id}**");
                    
                    replyBuilder.Append(type == HighscoreTypes.messages
                        ? $" - {value} {type} - {Math.Round((double) (100 * value) / guildMessages, 2)}%\n"
                        : $" - {value} {type}\n");

                    highscorePlace++;
                }

                await ReplyAsync(replyBuilder.ToString());
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}