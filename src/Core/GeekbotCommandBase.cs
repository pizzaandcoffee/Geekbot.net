using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.GuildSettingsManager;
using Sentry;

namespace Geekbot.Core
{
    public class GeekbotCommandBase : ModuleBase<ICommandContext>
    {
        protected readonly IGuildSettingsManager GuildSettingsManager;
        protected GuildSettingsModel GuildSettings;
        protected readonly IErrorHandler ErrorHandler;
        protected ITransaction Transaction;

        protected GeekbotCommandBase(IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager)
        {
            GuildSettingsManager = guildSettingsManager;
            ErrorHandler = errorHandler;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);

            // Transaction Setup
            Transaction = SentrySdk.StartTransaction(new Transaction(command.Name, "Exec"));
            Transaction.SetTags(new []
            {
                new KeyValuePair<string, string>("GuildId", Context.Guild.Id.ToString()),
                new KeyValuePair<string, string>("Guild", Context.Guild.Name),
            });
            Transaction.User = new User()
            {
                Id = Context.User.Id.ToString(),
                Username = Context.User.Username,
            };
            
            // Command Setup
            var setupSpan = Transaction.StartChild("Setup");
            
            GuildSettings = GuildSettingsManager.GetSettings(Context?.Guild?.Id ?? 0);
            var language = GuildSettings.Language;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(language);
            
            setupSpan.Finish();
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);
            Transaction.Finish();
        }

        protected override Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null)
        {
            var replySpan = Transaction.StartChild("Reply");
            var msg = base.ReplyAsync(message, isTTS, embed, options, allowedMentions, messageReference);
            replySpan.Finish();
            return msg;
        }
    }
}