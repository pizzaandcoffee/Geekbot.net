using System.Globalization;
using System.Threading;
using Discord.Commands;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.GuildSettingsManager;

namespace Geekbot.Core
{
    public class GeekbotCommandBase : TransactionModuleBase
    {
        protected readonly IGuildSettingsManager GuildSettingsManager;
        protected GuildSettingsModel GuildSettings;
        protected readonly IErrorHandler ErrorHandler;

        protected GeekbotCommandBase(IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager)
        {
            GuildSettingsManager = guildSettingsManager;
            ErrorHandler = errorHandler;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);

            var setupSpan = Transaction.StartChild("Setup");
            
            GuildSettings = GuildSettingsManager.GetSettings(Context?.Guild?.Id ?? 0);
            var language = GuildSettings.Language;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(language);
            
            setupSpan.Finish();
        }
    }
}