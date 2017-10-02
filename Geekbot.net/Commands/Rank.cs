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
    public class Rank : ModuleBase
    {
        private readonly IDatabase _redis;
        private readonly IErrorHandler _errorHandler;
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;
        
        public Rank(IDatabase redis, IErrorHandler errorHandler, ILogger logger, IUserRepository userRepository)
        {
            _redis = redis;
            _errorHandler = errorHandler;
            _logger = logger;
            _userRepository = userRepository;
        }
        
        [Command("rank", RunMode = RunMode.Async)]
        [Summary("get user top 10")]
        public async Task RankCmd()
        {
            try
            {
                var messageList = _redis.HashGetAll($"{Context.Guild.Id}:Messages");
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
                        var guildUser = _userRepository.Get((ulong)user.Name);
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
                        _logger.Warning(e, $"Could not retrieve user {user.Name}");
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
                _errorHandler.HandleCommandException(e, Context);
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
                _logger.Warning(e, $"Can't provide emoji number {number}");
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