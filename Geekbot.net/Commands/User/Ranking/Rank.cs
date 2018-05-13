using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Lib.Converters;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.UserRepository;
using StackExchange.Redis;

namespace Geekbot.net.Commands.User.Ranking
{
    public class Rank : ModuleBase
    {
        private readonly IEmojiConverter _emojiConverter;
        private readonly IErrorHandler _errorHandler;
        private readonly DatabaseContext _database;
        private readonly IUserRepository _userRepository;
        private readonly IDatabase _redis;

        public Rank(DatabaseContext database, IErrorHandler errorHandler, IUserRepository userRepository,
            IEmojiConverter emojiConverter, IDatabase redis)
        {
            _database = database;
            _errorHandler = errorHandler;
            _userRepository = userRepository;
            _emojiConverter = emojiConverter;
            _redis = redis;
        }

        [Command("rank", RunMode = RunMode.Async)]
        [Summary("get user top 10 in messages or karma")]
        public async Task RankCmd([Summary("type")] string typeUnformated = "messages", [Summary("amount")] int amount = 10)
        {
            try
            {
                RankType type;
                try
                {
                    type = Enum.Parse<RankType>(typeUnformated.ToLower());
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
                
                Dictionary<ulong, int> list;

                switch (type)
                {
                    case RankType.messages:
                        list = GetMessageList(amount);
                        break;
                    case RankType.karma:
                        list = GetKarmaList(amount);
                        break;
                    case RankType.rolls:
                        list = GetRollsList(amount);
                        break;
                    default:
                        await ReplyAsync("Valid types are '`messages`' '`karma`', '`rolls`'");
                        return;
                }

                if (!list.Any())
                {
                    await ReplyAsync($"No {type} found on this server");
                    return;
                }

                var highscoreUsers = new Dictionary<RankUserDto, int>();
                var failedToRetrieveUser = false;
                foreach (var user in list)
                {
                    try
                    {
                        var guildUser = _userRepository.Get(user.Key);
                        if (guildUser?.Username != null)
                        {
                            highscoreUsers.Add(new RankUserDto
                            {
                                Username = guildUser.Username,
                                Discriminator = guildUser.Discriminator
                            },  user.Value);
                        }
                        else
                        {
                            highscoreUsers.Add(new RankUserDto
                            {
                                Id = user.Key.ToString()
                            },  user.Value);
                            failedToRetrieveUser = true;
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                }

                if (failedToRetrieveUser) replyBuilder.AppendLine(":warning: Couldn't get all userdata\n");
                replyBuilder.AppendLine($":bar_chart: **{type} Highscore for {Context.Guild.Name}**");
                var highscorePlace = 1;
                foreach (var user in highscoreUsers)
                {
                    replyBuilder.Append(highscorePlace < 11
                        ? $"{_emojiConverter.NumberToEmoji(highscorePlace)} "
                        : $"`{highscorePlace}.` ");

                    replyBuilder.Append(user.Key.Username != null
                        ? $"**{user.Key.Username}#{user.Key.Discriminator}**"
                        : $"**{user.Key.Id}**");

                    replyBuilder.Append($" - {user.Value} {type}\n");

                    highscorePlace++;
                }

                await ReplyAsync(replyBuilder.ToString());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private Dictionary<ulong, int> GetMessageList(int amount)
        {
            return _redis
                .HashGetAll($"{Context.Guild.Id}:Messages").ToDictionary().Take(amount + 1)
                .Where(user => !user.Key.Equals(0))
                .ToDictionary(user => ulong.Parse(user.Key), user => int.Parse(user.Value));
        }
        
        private Dictionary<ulong, int> GetKarmaList(int amount)
        {
            return _database.Karma
                .Where(k => k.GuildId.Equals(Context.Guild.Id.AsLong()))
                .OrderByDescending(o => o.Karma)
                .Take(amount)
                .ToDictionary(key => key.UserId.AsUlong(), key => key.Karma);
        }
        
        private Dictionary<ulong, int> GetRollsList(int amount)
        {
            return _database.Rolls
                .Where(k => k.GuildId.Equals(Context.Guild.Id.AsLong()))
                .OrderByDescending(o => o.Rolls)
                .Take(amount)
                .ToDictionary(key => key.UserId.AsUlong(), key => key.Rolls);
        }
    }
}