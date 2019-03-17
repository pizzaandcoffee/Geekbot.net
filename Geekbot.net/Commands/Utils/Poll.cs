using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.CommandPreconditions;
using Geekbot.net.Lib.Converters;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.UserRepository;

namespace Geekbot.net.Commands.Utils
{
    [Group("poll")]
    [DisableInDirectMessage]
    public class Poll : ModuleBase
    {
        private readonly IEmojiConverter _emojiConverter;
        private readonly IErrorHandler _errorHandler;
        private readonly DatabaseContext _database;
        private readonly IUserRepository _userRepository;

        public Poll(IErrorHandler errorHandler, DatabaseContext database, IEmojiConverter emojiConverter, IUserRepository userRepository)
        {
            _errorHandler = errorHandler;
            _database = database;
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
                if (currentPoll.Question == null)
                {
                    await ReplyAsync(
                        "There is no poll in this channel ongoing at the moment\r\nYou can create one with `!poll create question;option1;option2;option3`");
                    return;
                }

                await ReplyAsync("There is a poll running at the moment");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("create", RunMode = RunMode.Async)]
        [Summary("Create a poll")]
        public async Task Create([Remainder] [Summary("question;option1;option2")]
            string rawPollString)
        {
            try
            {
                await ReplyAsync("Poll creation currently disabled");
                return;
                
//                var currentPoll = GetCurrentPoll();
//                if (currentPoll.Question != null && !currentPoll.IsFinshed)
//                {
//                    await ReplyAsync("You have not finished you last poll yet. To finish it use `!poll end`");
//                    return;
//                }
//
//                var pollList = rawPollString.Split(';').ToList();
//                if (pollList.Count <= 2)
//                {
//                    await ReplyAsync(
//                        "You need a question with atleast 2 options, a valid creation would look like this `question;option1;option2`");
//                    return;
//                }
//
//                var question = pollList[0];
//                pollList.RemoveAt(0);
//                
//                var eb = new EmbedBuilder
//                {
//                    Title = $"Poll by {Context.User.Username}",
//                    Description = question
//                };
//
//                var options = new List<PollQuestionModel>();
//                var i = 1;
//                pollList.ForEach(option =>
//                {
//                    options.Add(new PollQuestionModel()
//                    {
//                        OptionId = i,
//                        OptionText = option
//                    });
//                    eb.AddInlineField($"Option {_emojiConverter.NumberToEmoji(i)}", option);
//                    i++;
//                });
//                var pollMessage = await ReplyAsync("", false, eb.Build());
//                
//                var poll = new PollModel()
//                {
//                    Creator = Context.User.Id.AsLong(),
//                    MessageId = pollMessage.Id.AsLong(),
//                    IsFinshed = false,
//                    Question = question,
//                    Options = options
//                };
//                _database.Polls.Add(poll);
//
//                i = 1;
//                pollList.ForEach(option =>
//                {
//                    pollMessage.AddReactionAsync(new Emoji(_emojiConverter.NumberToEmoji(i)));
//                    Task.Delay(500);
//                    i++;
//                });
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
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

                currentPoll = await GetPollResults(currentPoll);
                var sb = new StringBuilder();
                sb.AppendLine("**Poll Results**");
                sb.AppendLine(currentPoll.Question);
                foreach (var result in currentPoll.Options) sb.AppendLine($"{result.Votes} - {result.OptionText}");
                await ReplyAsync(sb.ToString());
                currentPoll.IsFinshed = true;
                _database.Polls.Update(currentPoll);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        private PollModel GetCurrentPoll()
        {
            try
            {
                var currentPoll = _database.Polls.FirstOrDefault(poll =>
                    poll.ChannelId.Equals(Context.Channel.Id.AsLong()) &&
                    poll.GuildId.Equals(Context.Guild.Id.AsLong())
                );
                return currentPoll ?? new PollModel();

            }
            catch
            {
                return new PollModel();
            }
        }

        private async Task<PollModel> GetPollResults(PollModel poll)
        {
            var message = (IUserMessage) await Context.Channel.GetMessageAsync(poll.MessageId.AsUlong());
            
            var results = new Dictionary<int, int>();
            foreach (var r in message.Reactions)
            {
                try
                {
                    results.Add(r.Key.Name.ToCharArray()[0], r.Value.ReactionCount);
                }
                catch
                {
                    // ignored
                }
            }
            
            foreach (var q in poll.Options)
            {
                q.Votes = results.FirstOrDefault(e => e.Key.Equals(q.OptionId)).Value;
            }

            return poll;

//            var sortedValues = results.OrderBy(e => e.Value);
//            return sortedValues;
        }
    }
}