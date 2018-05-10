using System.Threading.Tasks;
using Discord.WebSocket;
using Geekbot.net.Database.Models;

namespace Geekbot.net.Lib.UserRepository
{
    public interface IUserRepository
    {
        Task<bool> Update(SocketUser user);
        UserModel Get(ulong userId);
    }
}