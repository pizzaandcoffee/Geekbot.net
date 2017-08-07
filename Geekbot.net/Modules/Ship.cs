﻿using System;
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

            var reply = ":heartpulse: **Matchmaking** :heartpulse:\r\n";
            reply = reply + $":two_hearts: {user1.Mention} :heart: {user2.Mention} :two_hearts:\r\n";
            reply = reply + $"0% [{BlockCounter(shippingRate)}] 100% - {DeterminateSuccess(shippingRate)}";
            await ReplyAsync(reply);
        }

        private string DeterminateSuccess(int rate)
        {
            if (rate < 20)
            {
                return "Not gonna happen";
            }  if (rate >= 20 && rate < 40)
            {
                return "Not such a good idea";
            }  if (rate >= 40 && rate < 60)
            {
                return "There might be a chance";
            }  if (rate >= 60 && rate < 80)
            {
                return "Almost a match, but could work";
            }  if (rate >= 80)
            {
                return "It's a match";
            }
            return "a";
        }

        private string BlockCounter(int rate)
        {
            var amount = Math.Floor(decimal.Floor(rate / 10));
            Console.WriteLine(amount);
            var blocks = "";
            for(int i = 1; i <= 10; i++)
            {
                if(i <= amount)
                {
                    blocks = blocks + ":white_medium_small_square:";
                    if(i == amount)
                    {
                        blocks = blocks + $" {rate}% ";
                    }
                } else
                {
                    blocks = blocks + ":black_medium_small_square:";
                }
            }
            return blocks;
        }
    }
}