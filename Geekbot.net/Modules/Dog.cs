using System;
using System.Threading.Tasks;
using Discord.Commands;
using RestSharp;

namespace Geekbot.net.Modules
{
    public class Dog : ModuleBase
    {
        [Command("dog", RunMode = RunMode.Async), Summary("Return a random image of a dog.")]
        public async Task Say()
        {
            var dogClient = new RestClient("http://random.dog");
            var request = new RestRequest("woof.json", Method.GET);
            Console.WriteLine(dogClient.BaseUrl);

            dogClient.ExecuteAsync<DogResponse>(request, async response => {
                await ReplyAsync(response.Data.url);
            });
        }
    }

    public class DogResponse
    {
        public string url { get; set; }
    }
}