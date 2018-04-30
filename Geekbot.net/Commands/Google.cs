using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using StackExchange.Redis;
using Utf8Json;

namespace Geekbot.net.Commands
{
    public class Google : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;

        public Google(IErrorHandler errorHandler, IDatabase redis)
        {
            _errorHandler = errorHandler;
            _redis = redis;
        }
        
        [Command("google", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Google Something.")]
        public async Task AskGoogle([Remainder, Summary("SearchText")] string searchText)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var apiKey = _redis.StringGet("googleGraphKey");
                    if (!apiKey.HasValue)
                    {
                        await ReplyAsync("No Google API key has been set, please contact my owner");
                        return;
                    } 
                    
                    var url = new Uri($"https://kgsearch.googleapis.com/v1/entities:search?languages=en&limit=1&query={searchText}&key={apiKey}");
                    var responseString = client.DownloadString(url);
                    var response = JsonSerializer.Deserialize<GoogleKgApiResponse>(responseString);

                    if (!response.ItemListElement.Any())
                    {
                        await ReplyAsync("No results were found...");
                        return;
                    }

                    var data = response.ItemListElement.First().Result;
                    var eb = new EmbedBuilder();
                    eb.Title = data.Name;
                    if(!string.IsNullOrEmpty(data.Description)) eb.WithDescription(data.Description);
                    if(!string.IsNullOrEmpty(data.DetailedDescription?.Url)) eb.WithUrl(data.DetailedDescription.Url);
                    if(!string.IsNullOrEmpty(data.DetailedDescription?.ArticleBody)) eb.AddField("Details", data.DetailedDescription.ArticleBody);
                    if(!string.IsNullOrEmpty(data.Image?.ContentUrl)) eb.WithThumbnailUrl(data.Image.ContentUrl);
                    
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        public class GoogleKgApiResponse
        {
            public List<GoogleKgApiElement> ItemListElement { get; set; }

            public class GoogleKgApiElement
            {
                public GoogleKgApiResult Result { get; set; }
                public double ResultScore { get; set; }
            }
            
            public class GoogleKgApiResult
            {
                public string Name { get; set; }
                public string Description { get; set; }
                public GoogleKgApiImage Image { get; set; }
                public GoogleKgApiDetailed DetailedDescription { get; set; }
            }

            public class GoogleKgApiImage
            {
                public string ContentUrl { get; set; }
                public string Url { get; set; }
            }

            public class GoogleKgApiDetailed
            {
                public string ArticleBody { get; set; }
                public string Url { get; set; }
                public string License { get; set; }
            }
        }
    }
}