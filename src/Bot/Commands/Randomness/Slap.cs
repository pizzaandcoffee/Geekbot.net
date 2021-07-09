using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;

namespace Geekbot.Bot.Commands.Randomness
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
        public async Task Slapper([Summary("@someone")] IUser user)
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
                    "powerless banhammer",
                    "piece of low fat mozzarella"
                };

                await ReplyAsync($"{Context.User.Username} slapped {user.Username} with a {things[new Random().Next(things.Count - 1)]}");
                
                await UpdateRecieved(user.Id);
                await UpdateGiven(Context.User.Id);
                await _database.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task UpdateGiven(ulong userId)
        {
            var user = await GetUser(userId);
            user.Given++;
            _database.Slaps.Update(user);
        }
        
        private async Task UpdateRecieved(ulong userId)
        {
            var user = await GetUser(userId);
            user.Recieved++;
            _database.Slaps.Update(user);
        }

        private async Task<SlapsModel> GetUser(ulong userId)
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
            await _database.SaveChangesAsync();
            return _database.Slaps.FirstOrDefault(e =>
                e.GuildId.Equals(Context.Guild.Id.AsLong()) &&
                e.UserId.Equals(userId.AsLong()));
        }
    }
}
