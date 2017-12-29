using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Geekbot.net.Lib;
using Newtonsoft.Json;
using StackExchange.Redis;

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
        public async Task askGoogle([Remainder, Summary("SearchText")] string searchText)
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
                    var response = Utf8Json.JsonSerializer.Deserialize<GoogleKGApiResponse>(responseString);

                    if (!response.itemListElement.Any())
                    {
                        await ReplyAsync("No results were found...");
                        return;
                    }

                    var data = response.itemListElement.First().result;
                    var eb = new EmbedBuilder();
                    eb.Title = data.name;
                    if(!string.IsNullOrEmpty(data.description)) eb.WithDescription(data.description);
                    if(!string.IsNullOrEmpty(data.detailedDescription?.url)) eb.WithUrl(data.detailedDescription.url);
                    if(!string.IsNullOrEmpty(data.detailedDescription?.articleBody)) eb.AddField("Details", data.detailedDescription.articleBody);
                    if(!string.IsNullOrEmpty(data.image?.contentUrl)) eb.WithThumbnailUrl(data.image.contentUrl);
                    
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        public class GoogleKGApiResponse
        {
            public List<GoogleKGApiElement> itemListElement { get; set; }

            public class GoogleKGApiElement
            {
                public GoogleKGApiResult result { get; set; }
                public double resultScore { get; set; }
            }
            
            public class GoogleKGApiResult
            {
                public string name { get; set; }
                public string description { get; set; }
                public GoogleKGApiImage image { get; set; }
                public GoogleKGApiDetailed detailedDescription { get; set; }
            }

            public class GoogleKGApiImage
            {
                public string contentUrl { get; set; }
                public string url { get; set; }
            }

            public class GoogleKGApiDetailed
            {
                public string articleBody { get; set; }
                public string url { get; set; }
                public string license { get; set; }
            }
        }
    }
}