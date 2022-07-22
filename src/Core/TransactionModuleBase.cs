using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Sentry;

namespace Geekbot.Core
{
    public class TransactionModuleBase : ModuleBase<ICommandContext>
    {
        protected ITransaction Transaction;

        protected override void BeforeExecute(CommandInfo command)
        {
            base.BeforeExecute(command);

            // Transaction Setup
            Transaction = SentrySdk.StartTransaction(new Transaction(command.Name, "Exec"));
            Transaction.SetTags(new []
            {
                new KeyValuePair<string, string>("Guild", Context.Guild.Name),
            });
            Transaction.User = new User()
            {
                Id = Context.User.Id.ToString(),
                Username = Context.User.Username,
            };
            Transaction.Status = SpanStatus.Ok;
        }

        protected override void AfterExecute(CommandInfo command)
        {
            base.AfterExecute(command);
            Transaction.Finish();
        }

        protected Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, AllowedMentions allowedMentions = null, MessageReference messageReference = null)
        {
            var replySpan = Transaction.StartChild("Reply");
            var msg = base.ReplyAsync(message, isTTS, embed, options, allowedMentions, messageReference);
            replySpan.Finish();
            return msg;
        }
    }
}