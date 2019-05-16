using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.GlobalSettings;
using Newtonsoft.Json;

namespace Geekbot.net.Commands.Integrations.Google
{
    public class Google : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IGlobalSettings _globalSettings;

        public Google(IErrorHandler errorHandler, IGlobalSettings globalSettings)
        {
            _errorHandler = errorHandler;
            _globalSettings = globalSettings;
        }
        
        [Command("google", RunMode = RunMode.Async)]
        [Summary("Google Something.")]
        public async Task AskGoogle([Remainder, Summary("search-text")] string searchText)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var apiKey = _globalSettings.GetKey("GoogleGraphKey");
                    if (string.IsNullOrEmpty(apiKey))
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

                    var data = response.ItemListElement.First().Result;
                    var eb = new EmbedBuilder
                    {
                        Title = data.Name
                    };
                    if(!string.IsNullOrEmpty(data.Description)) eb.WithDescription(data.Description);
                    if(!string.IsNullOrEmpty(data.DetailedDtoDescription?.Url)) eb.WithUrl(data.DetailedDtoDescription.Url);
                    if(!string.IsNullOrEmpty(data.DetailedDtoDescription?.ArticleBody)) eb.AddField("Details", data.DetailedDtoDescription.ArticleBody);
                    if(!string.IsNullOrEmpty(data.Image?.ContentUrl)) eb.WithThumbnailUrl(data.Image.ContentUrl);
                    
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}