﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Localization;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Games
{
    public class Roll : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;
        private readonly ITranslationHandler _translation;

        public Roll(IDatabase redis, IErrorHandler errorHandler, ITranslationHandler translation)
        {
            _redis = redis;
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
                var number = new Random().Next(1, 100);
                var guess = 1000;
                int.TryParse(stuff, out guess);
                var transDict = _translation.GetDict(Context);
                if (guess <= 100 && guess > 0)
                {
                    var prevRoll = _redis.HashGet($"{Context.Guild.Id}:RollsPrevious", Context.Message.Author.Id);
                    if (!prevRoll.IsNullOrEmpty && prevRoll.ToString() == guess.ToString())
                    {
                        await ReplyAsync(string.Format(transDict["NoPrevGuess"], Context.Message.Author.Mention));
                        return;
                    }

                    _redis.HashSet($"{Context.Guild.Id}:RollsPrevious",
                        new[] {new HashEntry(Context.Message.Author.Id, guess)});
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