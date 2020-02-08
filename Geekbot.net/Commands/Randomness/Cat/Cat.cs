using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;
using Newtonsoft.Json;

namespace Geekbot.net.Commands.Randomness.Cat
{
    public class Cat : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Cat(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("cat", RunMode = RunMode.Async)]
        [Summary("Return a random image of a cat.")]
        public async Task Say()
        {
            try
            {
                
                try
                {
                    using var client = new HttpClient
                    {
                        BaseAddress = new Uri("https://aws.random.cat")
                    };
                    var response = await client.GetAsync("/meow");
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    var catFile = JsonConvert.DeserializeObject<CatResponseDto>(stringResponse);
                    var eb = new EmbedBuilder
                    {
                        ImageUrl = catFile.File
                    };
                    await ReplyAsync("", false, eb.Build());
                }
                catch
                {
                    await ReplyAsync("Seems like the dog cought the cat (error occured)");
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}