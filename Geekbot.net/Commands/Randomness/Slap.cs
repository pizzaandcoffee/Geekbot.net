using System;
using System.Collections.Generic;
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
    public class Slap : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly DatabaseContext _database;

        public Slap(IErrorHandler errorHandler, DatabaseContext database)
        {
            _errorHandler = errorHandler;
            _database = database;
        }

        [Command("slap", RunMode = RunMode.Async)]
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
                
                var things = new List<string>
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
                    "bottle of rum",
                    "cheese slice",
                    "critical 1",
                    "natural 20",
                    "mjölnir (aka mewmew)",
                    "kamehameha",
                    "copy of Twilight",
                    "med pack (get ready for the end boss)",
                    "derp",
                    "condom (used)",
                    "gremlin fed after midnight",
                    "wet baguette",
                    "exploding kitten",
                    "shiny piece of shit",
                    "mismatched pair of socks",
                    "horcrux",
                    "tuna",
                    "suggestion",
                    "teapot",
                    "candle",
                    "dictionary",
                    "powerless banhammer"
                };

                await ReplyAsync($"{Context.User.Username} slapped {user.Username} with a {things[new Random().Next(things.Count - 1)]}");
                
                UpdateRecieved(user.Id);
                UpdateGiven(Context.User.Id);
                await _database.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private void UpdateGiven(ulong userId)
        {
            var user = GetUser(userId);
            user.Given++;
            _database.Slaps.Update(user);
        }
        
        private void UpdateRecieved(ulong userId)
        {
            var user = GetUser(userId);
            user.Recieved++;
            _database.Slaps.Update(user);
        }

        private SlapsModel GetUser(ulong userId)
        {
            var user = _database.Slaps.FirstOrDefault(e => 
                e.GuildId.Equals(Context.Guild.Id.AsLong()) &&
                e.UserId.Equals(userId.AsLong())
            );

            if (user != null) return user;
            
            _database.Slaps.Add(new SlapsModel
            {
                GuildId = Context.Guild.Id.AsLong(),
                UserId = userId.AsLong(),
                Recieved = 0,
                Given = 0
            });
            _database.SaveChanges();
            return _database.Slaps.FirstOrDefault(e =>
                e.GuildId.Equals(Context.Guild.Id.AsLong()) &&
                e.UserId.Equals(userId.AsLong()));
        }
    }
}