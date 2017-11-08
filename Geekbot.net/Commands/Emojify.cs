using System;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Emojify : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IEmojiConverter _emojiConverter;

        public Emojify(IErrorHandler errorHandler, IEmojiConverter emojiConverter)
        {
            _errorHandler = errorHandler;
            _emojiConverter = emojiConverter;
        }
        
        [Command("emojify", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Emojify text")]
        public async Task Dflt([Remainder, Summary("text")] string text)
        {
            try
            {
                var sb = new StringBuilder();
                await ReplyAsync($"*{Context.User.Username}#{Context.User.Discriminator} said:*");
                await ReplyAsync(_emojiConverter.textToEmoji(text));
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}