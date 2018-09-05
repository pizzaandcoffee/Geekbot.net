﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Lib.Converters;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Highscores;
using Geekbot.net.Lib.UserRepository;

namespace Geekbot.net.Commands.User.Ranking
{
    public class Rank : ModuleBase
    {
        private readonly IEmojiConverter _emojiConverter;
        private readonly IHighscoreManager _highscoreManager;
        private readonly IErrorHandler _errorHandler;
        private readonly DatabaseContext _database;
        private readonly IUserRepository _userRepository;

        public Rank(DatabaseContext database, IErrorHandler errorHandler, IUserRepository userRepository,
            IEmojiConverter emojiConverter, IHighscoreManager highscoreManager)
        {
            _database = database;
            _errorHandler = errorHandler;
            _userRepository = userRepository;
            _emojiConverter = emojiConverter;
            _highscoreManager = highscoreManager;
        }

        [Command("rank", RunMode = RunMode.Async)]
        [Summary("get user top 10 in messages or karma")]
        public async Task RankCmd([Summary("type")] string typeUnformated = "messages", [Summary("amount")] int amount = 10)
        {
            try
            {
                HighscoreTypes type;
                try
                {
                    type = Enum.Parse<HighscoreTypes>(typeUnformated.ToLower());
                }
                catch
                {
                    await ReplyAsync("Valid types are '`messages`' '`karma`', '`rolls`'");
                    return;
                }

                var replyBuilder = new StringBuilder();
                if (amount > 20)
                {
                    replyBuilder.AppendLine(":warning: Limiting to 20\n");
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
                    await ReplyAsync($"No {type} found on this server");
                    return;
                }

                int guildMessages = 0;
                if (type == HighscoreTypes.messages)
                {
                    guildMessages = _database.Messages
                        .Where(e => e.GuildId.Equals(Context.Guild.Id.AsLong()))
                        .Select(e => e.MessageCount)
                        .Sum();
                }

                var failedToRetrieveUser = highscoreUsers.Any(e => string.IsNullOrEmpty(e.Key.Username));

                if (failedToRetrieveUser) replyBuilder.AppendLine(":warning: I couldn't find all usernames. Maybe they left the server?\n");
                replyBuilder.AppendLine($":bar_chart: **{type.ToString().CapitalizeFirst()} Highscore for {Context.Guild.Name}**");
                var highscorePlace = 1;
                foreach (var user in highscoreUsers)
                {
                    replyBuilder.Append(highscorePlace < 11
                        ? $"{_emojiConverter.NumberToEmoji(highscorePlace)} "
                        : $"`{highscorePlace}.` ");

                    replyBuilder.Append(user.Key.Username != null
                        ? $"**{user.Key.Username}#{user.Key.Discriminator}**"
                        : $"**{user.Key.Id}**");
                    
                    replyBuilder.Append(type == HighscoreTypes.messages
                        ? $" - {user.Value} {type} - {Math.Round((double) (100 * user.Value) / guildMessages, digits: 2)}%\n"
                        : $" - {user.Value} {type}\n");

                    highscorePlace++;
                }

                await ReplyAsync(replyBuilder.ToString());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}