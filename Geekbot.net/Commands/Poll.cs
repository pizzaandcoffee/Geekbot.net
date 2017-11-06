using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    [Group("poll")]
    public class Poll : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;
        private readonly IEmojiConverter _emojiConverter;
        private readonly IUserRepository _userRepository;

        public Poll(IErrorHandler errorHandler, IDatabase redis, IEmojiConverter emojiConverter, IUserRepository userRepository)
        {
            _errorHandler = errorHandler;
            _redis = redis;
            _emojiConverter = emojiConverter;
            _userRepository = userRepository;
        }

        [Command(RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Check status of the current poll")]
        public async Task Dflt()
        {
            try
            {
                var currentPoll = GetCurrentPoll();
                if (currentPoll.Question == null)
                {
                    await ReplyAsync(
                        "There is no poll in this channel ongoing at the moment\r\nYou can create one with `!poll create question;option1;option2;option3`");
                    return;
                }
                var results = await getPollResults(currentPoll);

            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("create", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Create a poll")]
        public async Task Create([Remainder, Summary("question;option1;option2")] string rawPollString)
        {
            try
            {
                var pollList = rawPollString.Split(';').ToList();
                if (pollList.Count <= 2)
                {
                    await ReplyAsync(
                        "You need a question with atleast 2 options, a valid creation would look like this `question;option1;option2`");
                    return;
                }

                var eb = new EmbedBuilder();
                eb.Title = $":bar_chart: Poll by {Context.User.Username}";
                var question = pollList[0];
                eb.Description = question;
                pollList.RemoveAt(0);
                var i = 1;
                pollList.ForEach(option =>
                {
                    eb.AddInlineField($"Option {_emojiConverter.numberToEmoji(i)}", option);
                    i++;
                });
                var pollMessage = await ReplyAsync("", false, eb.Build());
                i = 1;
                pollList.ForEach(option =>
                {
                    pollMessage.AddReactionAsync(new Emoji(_emojiConverter.numberToEmoji(i)));
                    i++;
                });
                var poll = new PollData()
                {
                    Creator = Context.User.Id,
                    MessageId = pollMessage.Id,
                    IsFinshed = false,
                    Question = question,
                    Options = pollList
                };
                var pollJson = JsonConvert.SerializeObject(poll);
                _redis.HashSet($"{Context.Guild.Id}:Polls", new HashEntry[] {new HashEntry(Context.Channel.Id, pollJson)});
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command("end", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("End the current poll")]
        public async Task End()
        {
            try
            {
                var hasPoll = _redis.HashGet($"{Context.Guild.Id}:Polls", Context.Channel.Id);
                if (hasPoll.IsNullOrEmpty)
                {
                    await ReplyAsync(
                        "There is no poll in this channel ongoing at the moment\r\nYou can create one with `!poll create question;answer1;answer2;answer3`");
                    return;
                }
                
                
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private PollData GetCurrentPoll()
        {
            try
            {
                var currentPoll = _redis.HashGet($"{Context.Guild.Id}:Polls", Context.Channel.Id);
                return JsonConvert.DeserializeObject<PollData>(currentPoll.ToString());
            }
            catch 
            {
                return new PollData();
            }
        }

        private async Task<List<PollResult>> getPollResults(PollData poll)
        {
            var message = (IUserMessage)(await Context.Channel.GetMessageAsync(poll.MessageId));
            foreach (var r in message.Reactions)
            {
                Console.WriteLine($"{r.Key.Name}: {r.Value.ReactionCount}");
            }
            return new List<PollResult>();
        }

        private class PollData
        {
            public ulong Creator { get; set; }
            public ulong MessageId { get; set; }
            public bool IsFinshed { get; set; }
            public string Question { get; set; }
            public List<string> Options { get; set; }
        }
        
        private class PollResult
        {
            public string Option { get; set; }
            public string VoteCount { get; set; }
        }
    }
}