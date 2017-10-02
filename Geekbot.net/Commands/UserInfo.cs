using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class UserInfo : ModuleBase
    {
        private readonly IDatabase redis;
        private readonly IErrorHandler errorHandler;
        private readonly ILogger logger;
        private readonly IUserRepository userRepository;
        
        public UserInfo(IDatabase redis, IErrorHandler errorHandler, ILogger logger, IUserRepository userRepository)
        {
            this.redis = redis;
            this.errorHandler = errorHandler;
            this.logger = logger;
            this.userRepository = userRepository;
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

                var messages = (int) redis.HashGet($"{Context.Guild.Id}:Messages", userInfo.Id.ToString());
                var guildMessages = (int) redis.HashGet($"{Context.Guild.Id}:Messages", 0.ToString());
                var level = LevelCalc.GetLevelAtExperience(messages);

                var percent = Math.Round((double) (100 * messages) / guildMessages, 2);

                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(userInfo.GetAvatarUrl())
                    .WithName(userInfo.Username));
                eb.WithColor(new Color(221, 255, 119));
                
                var karma = redis.HashGet($"{Context.Guild.Id}:Karma", userInfo.Id);
                var correctRolls = redis.HashGet($"{Context.Guild.Id}:Rolls", userInfo.Id.ToString());

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
                errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("rank", RunMode = RunMode.Async)]
        [Summary("get user top 10")]
        public async Task Rank()
        {
            try
            {
                var messageList = redis.HashGetAll($"{Context.Guild.Id}:Messages");
                var sortedList = messageList.OrderByDescending(e => e.Value).ToList();
                var guildMessages = (int) sortedList.First().Value;
                sortedList.RemoveAt(0);

                var highscoreUsers = new Dictionary<RankUserPolyfill, int>();
                var listLimiter = 1;
                var failedToRetrieveUser = false;
                foreach (var user in sortedList)
                {
                    if (listLimiter > 10) break;
                    try
                    {
                        var guildUser = userRepository.Get((ulong)user.Name);
                        if (guildUser.Username != null)
                        {
                            highscoreUsers.Add(new RankUserPolyfill()
                            {
                                Username = guildUser.Username,
                                Discriminator = guildUser.Discriminator
                            }, (int) user.Value);
                        }
                        else
                        {
                            highscoreUsers.Add(new RankUserPolyfill()
                            {
                                Id = user.Name
                            }, (int) user.Value);
                            failedToRetrieveUser = true;
                        }
                        listLimiter++;
                    }
                    catch (Exception e)
                    {
                        logger.Warning(e, $"Could not retrieve user {user.Name}");
                    }
                    
                }
                
                var highScore = new StringBuilder();
                if (failedToRetrieveUser) highScore.AppendLine(":warning: I couldn't get all userdata, sorry! (bugfix coming soon:tm:)\n");
                highScore.AppendLine($":bar_chart: **Highscore for {Context.Guild.Name}**");
                var highscorePlace = 1;
                foreach (var user in highscoreUsers)
                {
                    var percent = Math.Round((double) (100 * user.Value) / guildMessages, 2);
                    if (user.Key.Username != null)
                    {
                        highScore.AppendLine(
                            $"{NumerToEmoji(highscorePlace)} **{user.Key.Username}#{user.Key.Discriminator}** - {percent}% of total - {user.Value} messages");
                    }
                    else
                    {
                        highScore.AppendLine(
                            $"{NumerToEmoji(highscorePlace)} **{user.Key.Id}** - {percent}% of total - {user.Value} messages");
                    }
                    highscorePlace++;
                }
                await ReplyAsync(highScore.ToString());
            }
            catch (Exception e)
            {
                errorHandler.HandleCommandException(e, Context);
            }
        }

        private string NumerToEmoji(int number)
        {
            var emojis = new string[] {":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:", ":eight:", ":nine:", ":keycap_ten:"};
            try
            {
                return emojis[number - 1];
            }
            catch (Exception e)
            {
                logger.Warning(e, $"Can't provide emoji number {number}");
                return ":zero:";
            }
        }
    }

    class RankUserPolyfill
    {
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Id { get; set; }
    }
}