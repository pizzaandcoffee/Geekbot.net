using System.Threading.Tasks;
using Discord.WebSocket;

namespace Geekbot.net.Lib.UserRepository
{
    public interface IUserRepository
    {
        Task<bool> Update(SocketUser user);
        UserRepositoryUser Get(ulong userId);
        string GetUserSetting(ulong userId, string setting);
        bool SaveUserSetting(ulong userId, string setting, string value);
    }
}