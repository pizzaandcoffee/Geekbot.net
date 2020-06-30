using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Commands.Randomness.Cat;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Newtonsoft.Json;

namespace Geekbot.net.Commands.Randomness.Greetings
{
    public class Greetings : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Greetings(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("hello", RunMode = RunMode.Async)]
        [Alias("greeting", "hi", "hallo")]
        [Summary("Say hello to the bot and get a reply in a random language")]
        public async Task GetGreeting()
        {
            try
            {
                var greeting = await GetRandomGreeting();

                var eb = new EmbedBuilder();
                eb.Title = greeting.Primary.Text;
                eb.AddInlineField("Language", greeting.Language);

                if (greeting.Primary.Dialect != null)
                {
                    eb.AddInlineField("Dialect", greeting.Primary.Dialect);
                }

                if (greeting.Primary.Romanization != null)
                {
                    eb.AddInlineField("Roman", greeting.Primary.Romanization);
                }

                await ReplyAsync(string.Empty, false, eb.Build());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task<GreetingBaseDto> GetRandomGreeting()
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://api.greetings.dev")
            };
            var response = await client.GetAsync("/v1/greeting");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GreetingBaseDto>(stringResponse);
        }
    }
}