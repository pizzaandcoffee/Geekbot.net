using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;

namespace Geekbot.Bot.Commands.Integrations
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
                var definition = await Geekbot.Commands.UrbanDictionary.UrbanDictionary.Run(word);
                if (definition == null)
                {
                    await ReplyAsync("That word hasn't been defined...");
                    return;
                }

                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder
                {
                    Name = definition.Word,
                    Url = definition.Permalink
                });
                var c = System.Drawing.Color.Gold;
                eb.WithColor(new Color(c.R, c.G, c.B));

                static string ShortenIfToLong(string str, int maxLength) => str.Length > maxLength ? $"{str.Substring(0, maxLength - 5)}[...]" : str;

                if (!string.IsNullOrEmpty(definition.Definition)) eb.Description = ShortenIfToLong(definition.Definition, 1800);
                if (!string.IsNullOrEmpty(definition.Example)) eb.AddField("Example", ShortenIfToLong(definition.Example, 1024));
                if (definition.ThumbsUp != 0) eb.AddInlineField("Upvotes", definition.ThumbsUp);
                if (definition.ThumbsDown != 0) eb.AddInlineField("Downvotes", definition.ThumbsDown);

                await ReplyAsync(string.Empty, false, eb.Build());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}