using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
namespace Geekbot.net.Commands
{
    public class Slap : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly Random _random;

        public Slap(IErrorHandler errorHandler, Random random)
        {
            _errorHandler = errorHandler;
            _random = random;
        }

        [Command("slap", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Fun)]
        [Summary("slap someone")]
        public async Task Slapper([Summary("@user")] IUser user)
        {
            try
            {
                var things = new List<string>()
                {
                    "thing",
                    "rubber duck",
                    "leek stick",
                    "large trout",
                    "flat hand",
                    "strip of bacon",
                    "feather",
                    "piece of pizza"
                };
                await ReplyAsync($"{Context.User.Username} slapped {user.Username} with a {things[_random.Next(things.Count - 1)]}");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}