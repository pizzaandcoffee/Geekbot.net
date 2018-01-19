using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class Slap : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly Random _random;
        private readonly IDatabase _redis;

        public Slap(IErrorHandler errorHandler, Random random, IDatabase redis)
        {
            _errorHandler = errorHandler;
            _random = random;
            _redis = redis;
        }

        [Command("slap", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Fun)]
        [Summary("slap someone")]
        public async Task Slapper([Summary("@user")] IUser user)
        {
            try
            {
                if (user.Id == Context.User.Id)
                {
                    await ReplyAsync("Why would you slap yourself?");
                    return;
                }
                
                var things = new List<string>()
                {
                    "thing",
                    "rubber chicken",
                    "leek stick",
                    "large trout",
                    "flat hand",
                    "strip of bacon",
                    "feather",
                    "piece of pizza",
                    "moldy banana",
                    "sharp retort",
                    "printed version of wikipedia",
                    "panda paw",
                    "spiked sledgehammer",
                    "monstertruck",
                    "dirty toilet brush",
                    "sleeping seagull",
                    "sunflower",
                    "mousepad",
                    "lolipop",
                    "bottle of rum"
                };
                
                _redis.HashIncrement($"{Context.Guild.Id}:SlapsRecieved", user.Id.ToString());
                _redis.HashIncrement($"{Context.Guild.Id}:SlapsGiven", Context.User.Id.ToString());
                
                await ReplyAsync($"{Context.User.Username} slapped {user.Username} with a {things[_random.Next(things.Count - 1)]}");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}