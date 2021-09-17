using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.RandomNumberGenerator;

namespace Geekbot.Bot.Commands.Randomness
{
    public class BenedictCumberbatchNameGenerator : TransactionModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public BenedictCumberbatchNameGenerator(IErrorHandler errorHandler, IRandomNumberGenerator randomNumberGenerator)
        {
            _errorHandler = errorHandler;
            _randomNumberGenerator = randomNumberGenerator;
        }

        [Command("bdcb", RunMode = RunMode.Async)]
        [Summary("Benedict Cumberbatch Name Generator")]
        public async Task GetQuote()
        {
            try
            {
                var firstnameList = new List<string>
                {
                    "Bumblebee", "Bandersnatch", "Broccoli", "Rinkydink", "Bombadil", "Boilerdang", "Bandicoot", "Fragglerock", "Muffintop", "Congleton", "Blubberdick", "Buffalo", "Benadryl",
                    "Butterfree", "Burberry", "Whippersnatch", "Buttermilk", "Beezlebub", "Budapest", "Boilerdang", "Blubberwhale", "Bumberstump", "Bulbasaur", "Cogglesnatch", "Liverswort",
                    "Bodybuild", "Johnnycash", "Bendydick", "Burgerking", "Bonaparte", "Bunsenburner", "Billiardball", "Bukkake", "Baseballmitt", "Blubberbutt", "Baseballbat", "Rumblesack",
                    "Barister", "Danglerack", "Rinkydink", "Bombadil", "Honkytonk", "Billyray", "Bumbleshack", "Snorkeldink", "Beetlejuice", "Bedlington", "Bandicoot", "Boobytrap", "Blenderdick",
                    "Bentobox", "Pallettown", "Wimbledon", "Buttercup", "Blasphemy", "Syphilis", "Snorkeldink", "Brandenburg", "Barbituate", "Snozzlebert", "Tiddleywomp", "Bouillabaisse",
                    "Wellington", "Benetton", "Bendandsnap", "Timothy", "Brewery", "Bentobox", "Brandybuck", "Benjamin", "Buckminster", "Bourgeoisie", "Bakery", "Oscarbait", "Buckyball",
                    "Bourgeoisie", "Burlington", "Buckingham", "Barnoldswick", "Bumblesniff", "Butercup", "Bubblebath", "Fiddlestick", "Bulbasaur", "Bumblebee", "Bettyboop", "Botany", "Cadbury",
                    "Brendadirk", "Buckingham", "Barnabus", "Barnacle", "Billybong", "Botany", "Benddadick", "Benderchick"
                };

                var lastnameList = new List<string>
                {
                    "Coddleswort", "Crumplesack", "Curdlesnoot", "Calldispatch", "Humperdinck", "Rivendell", "Cuttlefish", "Lingerie", "Vegemite", "Ampersand", "Cumberbund", "Candycrush",
                    "Clombyclomp", "Cragglethatch", "Nottinghill", "Cabbagepatch", "Camouflage", "Creamsicle", "Curdlemilk", "Upperclass", "Frumblesnatch", "Crumplehorn", "Talisman", "Candlestick",
                    "Chesterfield", "Bumbersplat", "Scratchnsniff", "Snugglesnatch", "Charizard", "Carrotstick", "Cumbercooch", "Crackerjack", "Crucifix", "Cuckatoo", "Cockletit", "Collywog",
                    "Capncrunch", "Covergirl", "Cumbersnatch", "Countryside", "Coggleswort", "Splishnsplash", "Copperwire", "Animorph", "Curdledmilk", "Cheddarcheese", "Cottagecheese", "Crumplehorn",
                    "Snickersbar", "Banglesnatch", "Stinkyrash", "Cameltoe", "Chickenbroth", "Concubine", "Candygram", "Moldyspore", "Chuckecheese", "Cankersore", "Crimpysnitch", "Wafflesmack",
                    "Chowderpants", "Toodlesnoot", "Clavichord", "Cuckooclock", "Oxfordshire", "Cumbersome", "Chickenstrips", "Battleship", "Commonwealth", "Cunningsnatch", "Custardbath",
                    "Kryptonite", "Curdlesnoot", "Cummerbund", "Coochyrash", "Crackerdong", "Crackerdong", "Curdledong", "Crackersprout", "Crumplebutt", "Colonist", "Coochierash", "Anglerfish",
                    "Cumbersniff", "Charmander", "Scratch-n-sniff", "Cumberbitch", "Pumpkinpatch", "Cramplesnutch", "Lumberjack", "Bonaparte", "Cul-de-sac", "Cankersore", "Cucumbercatch", "Contradict"
                };

                var lastname = lastnameList[_randomNumberGenerator.Next(0, lastnameList.Count - 1)];
                var firstname = firstnameList[_randomNumberGenerator.Next(0, firstnameList.Count - 1)];

                await ReplyAsync($"{firstname} {lastname}");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}
