using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.IClients;
using RestSharp;

namespace Geekbot.net.Modules
{
    public class Dog : ModuleBase
    {
        private readonly IDogClient dogClient;
        public Dog(IDogClient dogClient)
        {
            this.dogClient = dogClient;
        }

        [Command("dog", RunMode = RunMode.Async), Summary("Return a random image of a dog.")]
        public async Task Say()
        {
            var request = new RestRequest("woof.json", Method.GET);

            dynamic response = dogClient.Client.Execute<dynamic>(request);
            await ReplyAsync(response.Data["url"]);
        }
    }
}