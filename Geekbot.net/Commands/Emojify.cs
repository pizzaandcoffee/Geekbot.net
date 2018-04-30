using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Emojify : ModuleBase
    {
        private readonly IEmojiConverter _emojiConverter;
        private readonly IErrorHandler _errorHandler;

        public Emojify(IErrorHandler errorHandler, IEmojiConverter emojiConverter)
        {
            _errorHandler = errorHandler;
            _emojiConverter = emojiConverter;
        }

        [Command("emojify", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Emojify text")]
        public async Task Dflt([Remainder] [Summary("text")] string text)
        {
            try
            {
                var emojis = _emojiConverter.TextToEmoji(text);
                if (emojis.Length > 1999)
                {
                    await ReplyAsync("I can't take that much at once!");
                    return;
                }

                await ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} said:");
                await ReplyAsync(emojis);
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}