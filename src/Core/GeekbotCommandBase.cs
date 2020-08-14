using System.Globalization;
using System.Threading;
using Discord.Commands;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Localization;

namespace Geekbot.Core
{
    public class GeekbotCommandBase : ModuleBase<ICommandContext>
    {
        protected readonly IErrorHandler ErrorHandler;
        protected readonly ITranslationHandler Translations;

        protected GeekbotCommandBase(IErrorHandler errorHandler, ITranslationHandler translations)
        {
            ErrorHandler = errorHandler;
            Translations = translations;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);
            var language = Translations.GetServerLanguage(Context.Guild.Id);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(language == "CHDE" ? "de-ch" : language);
        }
    }
}