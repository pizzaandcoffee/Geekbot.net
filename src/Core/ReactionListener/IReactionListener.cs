using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Geekbot.Core.ReactionListener
{
    public interface IReactionListener
    {
        bool IsListener(ulong id);
        Task AddRoleToListener(ulong messageId, ulong guildId, string emoji, IRole role);
        void RemoveRole(ISocketMessageChannel channel, SocketReaction reaction);
        void GiveRole(ISocketMessageChannel message, SocketReaction reaction);
        IEmote ConvertStringToEmote(string emoji);
    }
}