using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Randomness
{
    public class Ship : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;

        public Ship(IDatabase redis, IErrorHandler errorHandler)
        {
            _redis = redis;
            _errorHandler = errorHandler;
        }

        [Command("Ship", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Fun)]
        [Summary("Ask the Shipping meter")]
        public async Task Command([Summary("@User1")] IUser user1, [Summary("@User2")] IUser user2)
        {
            try
            {
                var dbstring = "";
                if (user1.Id > user2.Id)
                    dbstring = $"{user1.Id}-{user2.Id}";
                else
                    dbstring = $"{user2.Id}-{user1.Id}";

                var dbval = _redis.HashGet($"{Context.Guild.Id}:Ships", dbstring);
                var shippingRate = 0;
                if (dbval.IsNullOrEmpty)
                {
                    shippingRate = new Random().Next(1, 100);
                    _redis.HashSet($"{Context.Guild.Id}:Ships", dbstring, shippingRate);
                }
                else
                {
                    shippingRate = int.Parse(dbval.ToString());
                }

                var reply = ":heartpulse: **Matchmaking** :heartpulse:\r\n";
                reply = reply + $":two_hearts: {user1.Mention} :heart: {user2.Mention} :two_hearts:\r\n";
                reply = reply + $"0% [{BlockCounter(shippingRate)}] 100% - {DeterminateSuccess(shippingRate)}";
                await ReplyAsync(reply);
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private string DeterminateSuccess(int rate)
        {
            if (rate < 20)
                return "Not gonna happen";
            if (rate >= 20 && rate < 40)
                return "Not such a good idea";
            if (rate >= 40 && rate < 60)
                return "There might be a chance";
            if (rate >= 60 && rate < 80)
                return "Almost a match, but could work";
            return rate >= 80 ? "It's a match" : "a";
        }

        private string BlockCounter(int rate)
        {
            var amount = Math.Floor(decimal.Floor(rate / 10));
            Console.WriteLine(amount);
            var blocks = "";
            for (var i = 1; i <= 10; i++)
                if (i <= amount)
                {
                    blocks = blocks + ":white_medium_small_square:";
                    if (i == amount)
                        blocks = blocks + $" {rate}% ";
                }
                else
                {
                    blocks = blocks + ":black_medium_small_square:";
                }

            return blocks;
        }
    }
}