using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.GuildSettingsManager;
using Localization = Geekbot.Core.Localization;

namespace Geekbot.Bot.Commands.Utils
{
    public class Choose : GeekbotCommandBase
    {
        public Choose(IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
        }

        [Command("choose", RunMode = RunMode.Async)]
        [Summary("Let the bot choose for you, separate options with a semicolon.")]
        public async Task Command([Remainder] [Summary("option1;option2")]
            string choices)
        {
            try
            {
                var choicesArray = choices.Split(';');
                var choice = new Random().Next(choicesArray.Length);
                await ReplyAsync(string.Format(Localization.Choose.Choice, choicesArray[choice].Trim()));
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}