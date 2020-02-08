using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;
using Newtonsoft.Json;

namespace Geekbot.net.Commands.Randomness.Kanye
{
    public class Kanye : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Kanye(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        [Command("kanye", RunMode = RunMode.Async)]
        [Summary("A random kayne west quote")]
        public async Task Say()
        {
            try
            {
                try
                {
                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    var response = await client.GetAsync("https://api.kanye.rest/");
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<KanyeResponseDto>(stringResponse);
                    await ReplyAsync(data.Quote);
                }
                catch (HttpRequestException)
                {
                    await ReplyAsync("Api down...");
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}