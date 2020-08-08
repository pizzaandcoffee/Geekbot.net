using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;

namespace Geekbot.Bot.Commands.Randomness.Greetings
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
                var greeting = await HttpAbstractions.Get<GreetingBaseDto>(new Uri("https://api.greetings.dev/v1/greeting"));

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
    }
}