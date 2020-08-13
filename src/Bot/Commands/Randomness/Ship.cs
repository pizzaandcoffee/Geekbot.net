using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.Localization;
using Geekbot.Core.RandomNumberGenerator;

namespace Geekbot.Bot.Commands.Randomness
{
    public class Ship : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ITranslationHandler _translation;
        private readonly DatabaseContext _database;

        public Ship(DatabaseContext database, IErrorHandler errorHandler, IRandomNumberGenerator randomNumberGenerator, ITranslationHandler translation)
        {
            _database = database;
            _errorHandler = errorHandler;
            _randomNumberGenerator = randomNumberGenerator;
            _translation = translation;
        }

        [Command("Ship", RunMode = RunMode.Async)]
        [Summary("Ask the Shipping meter")]
        public async Task Command([Summary("@user1")] IUser user1, [Summary("@user2")] IUser user2)
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
                    shippingRate = _randomNumberGenerator.Next(1, 100);
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

                var transContext = await _translation.GetGuildContext(Context);

                var reply = $":heartpulse: **{transContext.GetString("Matchmaking")}** :heartpulse:\r\n";
                reply += $":two_hearts: {user1.Mention} :heart: {user2.Mention} :two_hearts:\r\n";
                reply += $"0% [{BlockCounter(shippingRate)}] 100% - {DeterminateSuccess(shippingRate, transContext)}";
                await ReplyAsync(reply);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        private string DeterminateSuccess(int rate, TranslationGuildContext transContext)
        {
            return (rate / 20) switch
            {
                0 => transContext.GetString("NotGonnaToHappen"),
                1 => transContext.GetString("NotSuchAGoodIdea"),
                2 => transContext.GetString("ThereMightBeAChance"),
                3 => transContext.GetString("CouldWork"),
                4 => transContext.GetString("ItsAMatch"),
                _ => "nope"
            };
        }

        private string BlockCounter(int rate)
        {
            var amount = rate / 10;
            Console.WriteLine(amount);
            var blocks = "";
            for (var i = 1; i <= 10; i++)
                if (i <= amount)
                {
                    blocks += ":white_medium_small_square:";
                    if (i == amount)
                        blocks += $" {rate}% ";
                }
                else
                {
                    blocks += ":black_medium_small_square:";
                }

            return blocks;
        }
    }
}