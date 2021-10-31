using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.RandomNumberGenerator;
using Localization = Geekbot.Core.Localization;

namespace Geekbot.Bot.Commands.Randomness
{
    public class Ship : GeekbotCommandBase
    {
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly DatabaseContext _database;

        public Ship(DatabaseContext database, IErrorHandler errorHandler, IRandomNumberGenerator randomNumberGenerator, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
            _database = database;
            _randomNumberGenerator = randomNumberGenerator;
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

                var reply = $":heartpulse: **{Localization.Ship.Matchmaking}** :heartpulse:\r\n";
                reply += $":two_hearts: {user1.Mention} :heart: {user2.Mention} :two_hearts:\r\n";
                reply += $"0% [{BlockCounter(shippingRate)}] 100% - {DeterminateSuccess(shippingRate)}";
                await ReplyAsync(reply);
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        private string DeterminateSuccess(int rate)
        {
            return (rate / 20) switch
            {
                0 => Localization.Ship.NotGoingToHappen,
                1 => Localization.Ship.NotSuchAGoodIdea,
                2 => Localization.Ship.ThereMightBeAChance,
                3 => Localization.Ship.CouldWork,
                4 => Localization.Ship.ItsAMatch,
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