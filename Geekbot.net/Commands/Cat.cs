using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;

namespace Geekbot.net.Commands
{
    public class Cat : ModuleBase
    {
        [Command("cat", RunMode = RunMode.Async)]
        [Summary("Return a random image of a cat.")]
        public async Task Say()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://random.cat");
                    var response = await client.GetAsync("/meow.php");
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    var catFile = JsonConvert.DeserializeObject<CatResponse>(stringResponse);
                    await ReplyAsync(catFile.file);
                }
                catch (HttpRequestException e)
                {
                    await ReplyAsync($"Seems like the dog cought the cat (error occured)\r\n{e.Message}");
                }
            }
        }
    }

    public class CatResponse
    {
        public string file { get; set; }
    }
}