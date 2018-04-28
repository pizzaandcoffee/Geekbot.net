using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Newtonsoft.Json;

namespace Geekbot.net.Commands
{
    public class ChuckNorrisJokes : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public ChuckNorrisJokes(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        [Command("chuck", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Randomness)]
        [Summary("A random chuck norris joke")]
        public async Task Say()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                        var response = await client.GetAsync("https://api.chucknorris.io/jokes/random");
                        response.EnsureSuccessStatusCode();

                        var stringResponse = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<ChuckNorrisJokeResponse>(stringResponse);
                        await ReplyAsync(data.value);
                    }
                    catch (HttpRequestException)
                    {
                        await ReplyAsync("Api down...");
                    }
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private class ChuckNorrisJokeResponse
        {
            public string category { get; set; }
            public string icon_url { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public string value { get; set; }
        }
    }
}