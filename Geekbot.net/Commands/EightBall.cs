using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class EightBall : ModuleBase
    {
        private readonly Random _rnd;
        private readonly IErrorHandler _errorHandler;

        public EightBall(Random RandomClient, IErrorHandler errorHandler)
        {
            _rnd = RandomClient;
            _errorHandler = errorHandler;
        }

        [Command("8ball", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Randomness)]
        [Summary("Ask 8Ball a Question.")]
        public async Task Ball([Remainder] [Summary("Question")] string echo)
        {
            try
            {
                var replies = new List<string>
                {
                    "It is certain",
                    "It is decidedly so",
                    "Without a doubt",
                    "Yes, definitely",
                    "You may rely on it",
                    "As I see it, yes",
                    "Most likely",
                    "Outlook good",
                    "Yes",
                    "Signs point to yes",
                    "Reply hazy try again",
                    "Ask again later",
                    "Better not tell you now",
                    "Cannot predict now",
                    "Concentrate and ask again",
                    "Don't count on it",
                    "My reply is no",
                    "My sources say no",
                    "Outlook not so good",
                    "Very doubtful"
                };

                var answer = _rnd.Next(replies.Count);
                await ReplyAsync(replies[answer]);
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}