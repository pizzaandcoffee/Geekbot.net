using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Geekbot.Core.ReactionListener;

namespace Geekbot.Bot.Handlers
{
    public class ReactionHandler
    {
        private readonly IReactionListener _reactionListener;

        public ReactionHandler(IReactionListener reactionListener)
        {
            _reactionListener = reactionListener;
        }
        
        public Task Added(Cacheable<IUserMessage, ulong> cacheableUserMessage, Cacheable<IMessageChannel, ulong> cacheableMessageChannel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return Task.CompletedTask;
            if (!_reactionListener.IsListener(reaction.MessageId)) return Task.CompletedTask;
            _reactionListener.GiveRole(cacheableMessageChannel.Value, reaction);
            return Task.CompletedTask;
        }

        public Task Removed(Cacheable<IUserMessage, ulong> cacheableUserMessage, Cacheable<IMessageChannel, ulong> cacheableMessageChannel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return Task.CompletedTask;
            if (!_reactionListener.IsListener(reaction.MessageId)) return Task.CompletedTask;
            _reactionListener.RemoveRole(cacheableMessageChannel.Value, reaction);
            return Task.CompletedTask;
        }
    }
}