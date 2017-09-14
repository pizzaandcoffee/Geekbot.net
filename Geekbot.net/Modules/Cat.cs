using System;
using System.Threading.Tasks;
using Discord.Commands;
using RestSharp;

namespace Geekbot.net.Modules
{
    public class Cat : ModuleBase
    {
        [Command("cat", RunMode = RunMode.Async), Summary("Return a random image of a cat.")]
        public async Task Say()
        {
            var catClient = new RestClient("http://random.cat");
            var request = new RestRequest("meow.php", Method.GET);

            catClient.ExecuteAsync<CatResponse>(request, async response => {
                await ReplyAsync(response.Data.file);
            });
        }

    }

    public class CatResponse
    {
        public string file { get; set; }
    }
}