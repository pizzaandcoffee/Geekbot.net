using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;

namespace Geekbot.Bot.Commands.Integrations.UbranDictionary
{
    public class UrbanDictionary : TransactionModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public UrbanDictionary(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("urban", RunMode = RunMode.Async)]
        [Summary("Lookup something on urban dictionary")]
        public async Task UrbanDefine([Remainder] [Summary("word")] string word)
        {
            try
            {
                var definitions = await HttpAbstractions.Get<UrbanResponseDto>(new Uri($"https://api.urbandictionary.com/v0/define?term={word}"));
                if (definitions.List.Count == 0)
                {
                    await ReplyAsync("That word hasn't been defined...");
                    return;
                }

                var definition = definitions.List.First(e => !string.IsNullOrWhiteSpace(e.Example));

                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder
                {
                    Name = definition.Word,
                    Url = definition.Permalink
                });
                eb.WithColor(new Color(239, 255, 0));

                static string ShortenIfToLong(string str, int maxLength) => str.Length > maxLength ? $"{str.Substring(0, maxLength - 5)}[...]" : str;

                if (!string.IsNullOrEmpty(definition.Definition)) eb.Description = ShortenIfToLong(definition.Definition, 1800);
                if (!string.IsNullOrEmpty(definition.Example)) eb.AddField("Example", ShortenIfToLong(definition.Example, 1024));
                if (!string.IsNullOrEmpty(definition.ThumbsUp)) eb.AddInlineField("Upvotes", definition.ThumbsUp);
                if (!string.IsNullOrEmpty(definition.ThumbsDown)) eb.AddInlineField("Downvotes", definition.ThumbsDown);
                if (definitions.Tags?.Length > 0) eb.AddField("Tags", string.Join(", ", definitions.Tags));

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}