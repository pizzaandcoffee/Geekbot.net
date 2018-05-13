using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Converters;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.UserRepository;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Utils.Poll
{
    [Group("poll")]
    public class Poll : ModuleBase
    {
        private readonly IEmojiConverter _emojiConverter;
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;
        private readonly IUserRepository _userRepository;

        public Poll(IErrorHandler errorHandler, IDatabase redis, IEmojiConverter emojiConverter,
            IUserRepository userRepository)
        {
            _errorHandler = errorHandler;
            _redis = redis;
            _emojiConverter = emojiConverter;
            _userRepository = userRepository;
        }

        [Command(RunMode = RunMode.Async)]
        [Summary("Check status of the current poll")]
        public async Task Dflt()
        {
            try
            {
                var currentPoll = GetCurrentPoll();
                if (currentPoll.Question == null || currentPoll.IsFinshed)
                {
                    await ReplyAsync(
                        "There is no poll in this channel ongoing at the moment\r\nYou can create one with `!poll create question;option1;option2;option3`");
                    return;
                }

                await ReplyAsync("There is a poll running at the moment");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("create", RunMode = RunMode.Async)]
        [Summary("Create a poll")]
        public async Task Create([Remainder] [Summary("question;option1;option2")]
            string rawPollString)
        {
            try
            {
                var currentPoll = GetCurrentPoll();
                if (currentPoll.Question != null && !currentPoll.IsFinshed)
                {
                    await ReplyAsync("You have not finished you last poll yet. To finish it use `!poll end`");
                    return;
                }

                var pollList = rawPollString.Split(';').ToList();
                if (pollList.Count <= 2)
                {
                    await ReplyAsync(
                        "You need a question with atleast 2 options, a valid creation would look like this `question;option1;option2`");
                    return;
                }

                var eb = new EmbedBuilder();
                eb.Title = $"Poll by {Context.User.Username}";
                var question = pollList[0];
                eb.Description = question;
                pollList.RemoveAt(0);
                var i = 1;
                pollList.ForEach(option =>
                {
                    eb.AddInlineField($"Option {_emojiConverter.NumberToEmoji(i)}", option);
                    i++;
                });
                var pollMessage = await ReplyAsync("", false, eb.Build());
                i = 1;
                pollList.ForEach(option =>
                {
                    pollMessage.AddReactionAsync(new Emoji(_emojiConverter.NumberToEmoji(i)));
                    i++;
                });
                var poll = new PollDataDto
                {
                    Creator = Context.User.Id,
                    MessageId = pollMessage.Id,
                    IsFinshed = false,
                    Question = question,
                    Options = pollList
                };
                var pollJson = JsonConvert.SerializeObject(poll);
                _redis.HashSet($"{Context.Guild.Id}:Polls", new[] {new HashEntry(Context.Channel.Id, pollJson)});
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("end", RunMode = RunMode.Async)]
        [Summary("End the current poll")]
        public async Task End()
        {
            try
            {
                var currentPoll = GetCurrentPoll();
                if (currentPoll.Question == null || currentPoll.IsFinshed)
                {
                    await ReplyAsync("There is no ongoing poll at the moment");
                    return;
                }

                var results = await GetPollResults(currentPoll);
                var sb = new StringBuilder();
                sb.AppendLine("**Poll Results**");
                sb.AppendLine(currentPoll.Question);
                foreach (var result in results) sb.AppendLine($"{result.VoteCount} - {result.Option}");
                await ReplyAsync(sb.ToString());
                currentPoll.IsFinshed = true;
                var pollJson = JsonConvert.SerializeObject(currentPoll);
                _redis.HashSet($"{Context.Guild.Id}:Polls", new[] {new HashEntry(Context.Channel.Id, pollJson)});
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private PollDataDto GetCurrentPoll()
        {
            try
            {
                var currentPoll = _redis.HashGet($"{Context.Guild.Id}:Polls", Context.Channel.Id);
                return JsonConvert.DeserializeObject<PollDataDto>(currentPoll.ToString());
            }
            catch
            {
                return new PollDataDto();
            }
        }

        private async Task<List<PollResultDto>> GetPollResults(PollDataDto poll)
        {
            var message = (IUserMessage) await Context.Channel.GetMessageAsync(poll.MessageId);
            var results = new List<PollResultDto>();
            foreach (var r in message.Reactions)
                try
                {
                    var option = int.Parse(r.Key.Name.ToCharArray()[0].ToString());
                    var result = new PollResultDto
                    {
                        Option = poll.Options[option - 1],
                        VoteCount = r.Value.ReactionCount
                    };
                    results.Add(result);
                }
                catch {}

            results.Sort((x, y) => y.VoteCount.CompareTo(x.VoteCount));
            return results;
        }
    }
}