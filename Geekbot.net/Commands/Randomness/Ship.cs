using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;

namespace Geekbot.net.Commands.Randomness
{
    public class Ship : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly DatabaseContext _database;

        public Ship(DatabaseContext database, IErrorHandler errorHandler)
        {
            _database = database;
            _errorHandler = errorHandler;
        }

        [Command("Ship", RunMode = RunMode.Async)]
        [Summary("Ask the Shipping meter")]
        public async Task Command([Summary("@User1")] IUser user1, [Summary("@User2")] IUser user2)
        {
            try
            {
                var userKeys = user1.Id < user2.Id
                    ? new Tuple<long, long>(user1.Id.AsLong(), user2.Id.AsLong())
                    : new Tuple<long, long>(user2.Id.AsLong(), user1.Id.AsLong());

                var dbval = _database.Ships.FirstOrDefault(s =>
                    s.FirstUserId.Equals(userKeys.Item1) &&
                    s.SecondUserId.Equals(userKeys.Item2));
                
                var shippingRate = 0;
                if (dbval == null)
                {
                    shippingRate = new Random().Next(1, 100);
                    _database.Ships.Add(new ShipsModel()
                    {
                        FirstUserId = userKeys.Item1,
                        SecondUserId = userKeys.Item2,
                        Strength = shippingRate
                    });
                    await _database.SaveChangesAsync();
                }
                else
                {
                    shippingRate = dbval.Strength;
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