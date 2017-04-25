using System;
using System.Threading.Tasks;
using Discord.Commands;
using RestSharp;

namespace Geekbot.net.Modules
{
    public class Cat : ModuleBase
    {
        private readonly ICatClient catClient;
        public Cat(ICatClient catClient)
        {
            this.catClient = catClient;
        }

        [Command("cat", RunMode = RunMode.Async), Summary("Return a random image of a cat.")]
        public async Task Say()
        {
            var request = new RestRequest("meow.php", Method.GET);

            dynamic response = catClient.Client.Execute<dynamic>(request);
            await ReplyAsync(response.Data["file"]);
        }
    }
}