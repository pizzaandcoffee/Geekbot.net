using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Geekbot.Core.ReactionListener
{
    public interface IReactionListener
    {
        bool IsListener(ulong id);
        Task AddRoleToListener(ulong messageId, ulong guildId, string emoji, IRole role);
        void RemoveRole(IMessageChannel channel, SocketReaction reaction);
        void GiveRole(IMessageChannel message, SocketReaction reaction);
        IEmote ConvertStringToEmote(string emoji);
    }
}