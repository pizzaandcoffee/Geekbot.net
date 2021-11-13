using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.Converters;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Utils
{
    public class Emojify : TransactionModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Emojify(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("emojify", RunMode = RunMode.Async)]
        [Summary("Emojify text")]
        public async Task Dflt([Remainder] [Summary("text")] string text)
        {
            try
            {
                var emojis = EmojiConverter.TextToEmoji(text);
                if (emojis.Length > 1999)
                {
                    await ReplyAsync("I can't take that much at once!");
                    return;
                }

                await ReplyAsync($"{Context.User.Username}#{Context.User.Discriminator} said:");
                await ReplyAsync(emojis);
                try
                {
                    await Context.Message.DeleteAsync();
                }
                catch
                {
                    // bot may not have enough permission, doesn't matter if it fails
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}