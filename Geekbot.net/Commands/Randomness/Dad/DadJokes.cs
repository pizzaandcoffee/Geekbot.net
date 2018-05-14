using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;
using Newtonsoft.Json;

namespace Geekbot.net.Commands.Randomness.Dad
{
    public class DadJokes : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public DadJokes(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        [Command("dad", RunMode = RunMode.Async)]
        [Summary("A random dad joke")]
        public async Task Say()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                        var response = await client.GetAsync("https://icanhazdadjoke.com/");
                        response.EnsureSuccessStatusCode();

                        var stringResponse = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<DadJokeResponseDto>(stringResponse);
                        await ReplyAsync(data.Joke);
                    }
                    catch (HttpRequestException)
                    {
                        await ReplyAsync("Api down...");
                    }
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}