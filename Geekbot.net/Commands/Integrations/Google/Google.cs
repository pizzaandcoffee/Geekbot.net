using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Integrations.Google
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
                    var response = JsonConvert.DeserializeObject<GoogleKgApiResponseDto>(responseString);

                    if (!response.ItemListElement.Any())
                    {
                        await ReplyAsync("No results were found...");
                        return;
                    }

                    var data = response.ItemListElement.First().ResultDto;
                    var eb = new EmbedBuilder();
                    eb.Title = data.Name;
                    if(!string.IsNullOrEmpty(data.Description)) eb.WithDescription(data.Description);
                    if(!string.IsNullOrEmpty(data.DetailedDtoDescription?.Url)) eb.WithUrl(data.DetailedDtoDescription.Url);
                    if(!string.IsNullOrEmpty(data.DetailedDtoDescription?.ArticleBody)) eb.AddField("Details", data.DetailedDtoDescription.ArticleBody);
                    if(!string.IsNullOrEmpty(data.ImageDto?.ContentUrl)) eb.WithThumbnailUrl(data.ImageDto.ContentUrl);
                    
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}