using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Newtonsoft.Json;

namespace Geekbot.net.Commands
{
    public class Dog : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        
        public Dog(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        [Command("dog", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Randomness)]
        [Summary("Return a random image of a dog.")]
        public async Task Say()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri("http://random.dog");
                        var response = await client.GetAsync("/woof.json");
                        response.EnsureSuccessStatusCode();

                        var stringResponse = await response.Content.ReadAsStringAsync();
                        var dogFile = JsonConvert.DeserializeObject<DogResponse>(stringResponse);
                        await ReplyAsync(dogFile.url);
                    }
                    catch (HttpRequestException e)
                    {
                        await ReplyAsync($"Seems like the dog got lost (error occured)\r\n{e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }

    public class DogResponse
    {
        public string url { get; set; }
    }
}