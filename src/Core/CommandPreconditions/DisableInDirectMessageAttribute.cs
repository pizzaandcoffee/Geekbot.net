using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.Core.CommandPreconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class DisableInDirectMessageAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var result = context.Guild.Id != 0 ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("Command unavailable in Direct Messaging");
            return Task.FromResult(result);
        }
    }
}