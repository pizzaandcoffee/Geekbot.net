using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class Rank : ModuleBase
    {
        private readonly IEmojiConverter _emojiConverter;
        private readonly IErrorHandler _errorHandler;
        private readonly IGeekbotLogger _logger;
        private readonly IDatabase _redis;
        private readonly IUserRepository _userRepository;
        private readonly DiscordSocketClient _client;

        public Rank(IDatabase redis, IErrorHandler errorHandler, IGeekbotLogger logger, IUserRepository userRepository,
            IEmojiConverter emojiConverter, DiscordSocketClient client)
        {
            _redis = redis;
            _errorHandler = errorHandler;
            _logger = logger;
            _userRepository = userRepository;
            _emojiConverter = emojiConverter;
            _client = client;
        }

        [Command("rank", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Statistics)]
        [Summary("get user top 10 in messages or karma")]
        public async Task RankCmd([Summary("type")] string typeUnformated = "messages",
            [Summary("amount")] int amount = 10)
        {
            try
            {
                var type = typeUnformated.ToCharArray().First().ToString().ToUpper() + typeUnformated.Substring(1);

                if (!type.Equals("Messages") && !type.Equals("Karma") && !type.Equals("Rolls"))
                {
                    await ReplyAsync("Valid types are '`messages`' '`karma`', '`rolls`'");
                    return;
                }

                var replyBuilder = new StringBuilder();

                if (amount > 20)
                {
                    replyBuilder.AppendLine(":warning: Limiting to 20");
                    amount = 20;
                }

                var messageList = _redis.HashGetAll($"{Context.Guild.Id}:{type}");
                if (messageList.Length == 0)
                {
                    await ReplyAsync($"No {type.ToLowerInvariant()} found on this server");
                    return;
                }
                var sortedList = messageList.OrderByDescending(e => e.Value).ToList();
                var guildMessages = (int) sortedList.First().Value;
                var theBot = sortedList.FirstOrDefault(e => e.Name.ToString().Equals(_client.CurrentUser.Id.ToString()));
                if (!string.IsNullOrEmpty(theBot.Name))
                {
                    sortedList.Remove(theBot);
                }
                if (type == "Messages") sortedList.RemoveAt(0);

                var highscoreUsers = new Dictionary<RankUserPolyfill, int>();
                var listLimiter = 1;
                var failedToRetrieveUser = false;
                foreach (var user in sortedList)
                {
                    if (listLimiter > amount) break;
                    try
                    {
                        var guildUser = _userRepository.Get((ulong) user.Name);
                        if (guildUser.Username != null)
                        {
                            highscoreUsers.Add(new RankUserPolyfill
                            {
                                Username = guildUser.Username,
                                Discriminator = guildUser.Discriminator
                            }, (int) user.Value);
                        }
                        else
                        {
                            highscoreUsers.Add(new RankUserPolyfill
                            {
                                Id = user.Name
                            }, (int) user.Value);
                            failedToRetrieveUser = true;
                        }

                        listLimiter++;
                    }
                    catch (Exception e)
                    {
                        _logger.Warning("Geekbot", $"Could not retrieve user {user.Name}", e);
                    }
                }

                if (failedToRetrieveUser) replyBuilder.AppendLine(":warning: Couldn't get all userdata\n");
                replyBuilder.AppendLine($":bar_chart: **{type} Highscore for {Context.Guild.Name}**");
                var highscorePlace = 1;
                foreach (var user in highscoreUsers)
                {
                    replyBuilder.Append(highscorePlace < 11
                        ? $"{_emojiConverter.numberToEmoji(highscorePlace)} "
                        : $"`{highscorePlace}.` ");

                    replyBuilder.Append(user.Key.Username != null
                        ? $"**{user.Key.Username}#{user.Key.Discriminator}**"
                        : $"**{user.Key.Id}**");

                    switch (type)
                    {
                        case "Messages":
                            var percent = Math.Round((double) (100 * user.Value) / guildMessages, 2);
                            replyBuilder.Append($" - {percent}% of total - {user.Value} messages");
                            break;
                        case "Karma":
                            replyBuilder.Append($" - {user.Value} Karma");
                            break;
                        case "Rolls":
                            replyBuilder.Append($" - {user.Value} Guessed");
                            break;
                    }

                    replyBuilder.Append("\n");

                    highscorePlace++;
                }

                await ReplyAsync(replyBuilder.ToString());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }

    internal class RankUserPolyfill
    {
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Id { get; set; }
    }
}