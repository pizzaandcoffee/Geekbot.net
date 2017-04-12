using System;
using System.Threading.Tasks;
using Discord.Commands;
using RestSharp;

namespace Geekbot.net.Modules
{
    public class Cat : ModuleBase
    {
        [Command("cat"), Summary("Return a random image of a cat.")]
        public async Task Say()
        {
            var client = new RestClient("http://random.cat");

            var request = new RestRequest("meow.php", Method.GET);

            var response = client.Execute<CatObject>(request);
            await ReplyAsync(response.Data.file);
        }
    }

    public class CatObject
    {
        public string file {get;set;}
    }
}