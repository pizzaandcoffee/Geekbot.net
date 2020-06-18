using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;

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
                var greeting = GreetingProvider.Greetings[new Random().Next(GreetingProvider.GreetingCount - 1)];

                var eb = new EmbedBuilder();
                eb.Title = greeting.Text;
                eb.AddInlineField("Language", greeting.Language);

                if (greeting.Dialect != null)
                {
                    eb.AddInlineField("Dialect", greeting.Dialect);
                }

                if (greeting.Romanization != null)
                {
                    eb.AddInlineField("Roman", greeting.Romanization);
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