using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Trump : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        
        public Trump(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("trump", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Games)]
        [Summary("Get a trump quote")]
        public async Task getQuote()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var url = new Uri("https://api.tronalddump.io/random/quote");
                    var response = client.DownloadString(url);
                    var quote = Utf8Json.JsonSerializer.Deserialize<TrumpQuote>(response);

                    var eb = new EmbedBuilder();
                    eb.WithTitle($"Trump @ {quote.appeared_at.Day}.{quote.appeared_at.Month}.{quote.appeared_at.Year}");
                    eb.WithDescription(quote.value);
                    eb.WithUrl(quote._embedded.source.FirstOrDefault()?.url.ToString());
                    eb.WithColor(new Color(143, 167, 232));
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        public class TrumpQuote
        {
            public DateTimeOffset appeared_at { get; set; }
            public DateTimeOffset created_at { get; set; }
            public string quote_id { get; set; }
            public DateTimeOffset updated_at { get; set; }
            public string value { get; set; }
            public Embedded _embedded { get; set; }
            
            public class Embedded
            {
                public List<Source> source { get; set; }
                
                public class Source
                {
                    public DateTimeOffset created_at { get; set; }
                    public string filename { get; set; }
                    public string quote_source_id { get; set; }
                    public string remarks { get; set; }
                    public DateTimeOffset updated_at { get; set; }
                    public Uri url { get; set; }
                }
            }
        }
    }
}