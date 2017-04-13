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

        [Command("cat"), Summary("Return a random image of a cat.")]
        public async Task Say()
        {
            var request = new RestRequest("meow.php", Method.GET);

            var response = catClient.Client.Execute<CatObject>(request);
            await ReplyAsync(response.Data.file);
        }
    }

    public class CatObject
    {
        public string file {get;set;}
    }
}