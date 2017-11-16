using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class Roll : ModuleBase
    {
        private readonly IDatabase _redis;
        private readonly Random _rnd;
        private readonly ITranslationHandler _translation;
        private readonly IErrorHandler _errorHandler;

        public Roll(IDatabase redis, Random RandomClient, IErrorHandler errorHandler, ITranslationHandler translation)
        {
            _redis = redis;
            _rnd = RandomClient;
            _translation = translation;
            _errorHandler = errorHandler;
        }

        [Command("roll", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Fun)]
        [Summary("Guess which number the bot will roll (1-100")]
        public async Task RollCommand([Remainder] [Summary("guess")] string stuff = "noGuess")
        {
            try
            {
                var number = _rnd.Next(1, 100);
                var guess = 1000;
                int.TryParse(stuff, out guess);
                var transDict = _translation.GetDict(Context);
                if (guess <= 100 && guess > 0)
                {
                    await ReplyAsync(string.Format(transDict["Rolled"], Context.Message.Author.Mention, number, guess));
                    if (guess == number)
                    {
                        await ReplyAsync(string.Format(transDict["Gratz"], Context.Message.Author));
                        _redis.HashIncrement($"{Context.Guild.Id}:Rolls", Context.User.Id.ToString());
                    }
                }
                else
                {
                    await ReplyAsync(string.Format(transDict["RolledNoGuess"], Context.Message.Author.Mention, number));
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}