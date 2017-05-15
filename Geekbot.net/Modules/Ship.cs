using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib.IClients;

namespace Geekbot.net.Modules
{
    public class Ship : ModuleBase
    {

        private readonly IRedisClient redis;
        private readonly IRandomClient rnd;
        public Ship(IRedisClient redisClient, IRandomClient randomClient)
        {
            redis = redisClient;
            rnd = randomClient;
        }

        [Command("Ship", RunMode = RunMode.Async), Summary("Ask the Shipping meter")]
        public async Task Command([Summary("User 1")] IUser user1, [Summary("User 2")] IUser user2)
        {
            // Create a String
            var dbstring = "";
            if (user1.Id > user2.Id)
            {
                dbstring = $"{user1.Id}-{user2.Id}";
            }
            else
            {
                dbstring = $"{user2.Id}-{user1.Id}";
            }
            dbstring = $"{Context.Guild.Id}-{dbstring}";
            Console.WriteLine(dbstring);

            var dbval = redis.Client.StringGet(dbstring);
            var shippingRate = 0;
            if (dbval.IsNullOrEmpty)
            {
                shippingRate = rnd.Client.Next(1, 100);
                redis.Client.StringSet(dbstring, shippingRate);
            }
            else
            {
                shippingRate = int.Parse(dbval.ToString());
            }

            var reply = "";
            reply = reply + $"{user1.Username} :heart: {user2.Username}\r\n";
            reply = reply + $"0% [----{shippingRate}%----] 100% - {determinateSuccess(shippingRate)}";
            ReplyAsync(reply);
        }

        private string determinateSuccess(int rate)
        {
            if (rate < 20)
            {
                return "Not gonna happen";
            }  if (rate >= 20 && rate < 40)
            {
                return "A slight chance";
            }  if (rate >= 40 && rate < 60)
            {
                return "Perhaps it could work";
            }  if (rate >= 60 && rate < 80)
            {
                return "A good match";
            }  if (rate >= 80)
            {
                return "10/10";
            }
            return "a";
        }
    }
}