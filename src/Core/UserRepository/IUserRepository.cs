using System.Threading.Tasks;
using Discord.WebSocket;
using Geekbot.Core.Database.Models;

namespace Geekbot.Core.UserRepository
{
    public interface IUserRepository
    {
        Task<bool> Update(SocketUser user);
        UserModel Get(ulong userId);
    }
}