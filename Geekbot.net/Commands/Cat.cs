using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Newtonsoft.Json;

namespace Geekbot.net.Commands
{
    public class Cat : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Cat(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("cat", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Randomness)]
        [Summary("Return a random image of a cat.")]
        public async Task Say()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri("https://aws.random.cat");
                        var response = await client.GetAsync("/meow");
                        response.EnsureSuccessStatusCode();

                        var stringResponse = await response.Content.ReadAsStringAsync();
                        var catFile = JsonConvert.DeserializeObject<CatResponse>(stringResponse);
                        var eb = new EmbedBuilder();
                        eb.ImageUrl = catFile.file;
                        await ReplyAsync("", false, eb.Build());
                    }
                    catch
                    {
                        await ReplyAsync("Seems like the dog cought the cat (error occured)");
                    }
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private class CatResponse
        {
            public string file { get; set; }
        }
    }
}