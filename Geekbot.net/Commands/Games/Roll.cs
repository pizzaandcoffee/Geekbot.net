using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.AlmostRedis;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Localization;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Games
{
    public class Roll : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IAlmostRedis _redis;
        private readonly ITranslationHandler _translation;

        public Roll(IAlmostRedis redis, IErrorHandler errorHandler, ITranslationHandler translation)
        {
            _redis = redis;
            _translation = translation;
            _errorHandler = errorHandler;
        }

        [Command("roll", RunMode = RunMode.Async)]
        [Summary("Guess which number the bot will roll (1-100")]
        public async Task RollCommand([Remainder] [Summary("guess")] string stuff = "noGuess")
        {
            try
            {
                var number = new Random().Next(1, 100);
                var guess = 1000;
                int.TryParse(stuff, out guess);
                var transDict = await _translation.GetDict(Context);
                if (guess <= 100 && guess > 0)
                {
                    var prevRoll = _redis.Db.HashGet($"{Context.Guild.Id}:RollsPrevious2", Context.Message.Author.Id).ToString()?.Split('|');
                    if (prevRoll?.Length == 2)
                    {
                        if (prevRoll[0] == guess.ToString() && DateTime.Parse(prevRoll[1]) > DateTime.Now.AddDays(-1))
                        {
                            await ReplyAsync(string.Format(transDict["NoPrevGuess"], Context.Message.Author.Mention));
                            return;
                        }
                    }

                    _redis.Db.HashSet($"{Context.Guild.Id}:RollsPrevious2", new[] {new HashEntry(Context.Message.Author.Id, $"{guess}|{DateTime.Now}")});

                    await ReplyAsync(string.Format(transDict["Rolled"], Context.Message.Author.Mention, number, guess));
                    if (guess == number)
                    {
                        await ReplyAsync(string.Format(transDict["Gratz"], Context.Message.Author));
                        _redis.Db.HashIncrement($"{Context.Guild.Id}:Rolls", Context.User.Id.ToString());
                    }
                }
                else
                {
                    await ReplyAsync(string.Format(transDict["RolledNoGuess"], Context.Message.Author.Mention, number));
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}