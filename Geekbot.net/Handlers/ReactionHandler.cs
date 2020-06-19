using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Geekbot.net.Lib.ReactionListener;

namespace Geekbot.net.Handlers
{
    public class ReactionHandler
    {
        private readonly IReactionListener _reactionListener;

        public ReactionHandler(IReactionListener reactionListener)
        {
            _reactionListener = reactionListener;
        }
        
        public Task Added(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return Task.CompletedTask;
            if (!_reactionListener.IsListener(reaction.MessageId)) return Task.CompletedTask;
            _reactionListener.GiveRole(socketMessageChannel, reaction);
            return Task.CompletedTask;
        }

        public Task Removed(Cacheable<IUserMessage, ulong> cacheable, ISocketMessageChannel socketMessageChannel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return Task.CompletedTask;
            if (!_reactionListener.IsListener(reaction.MessageId)) return Task.CompletedTask;
            _reactionListener.RemoveRole(socketMessageChannel, reaction);
            return Task.CompletedTask;
        }
    }
}