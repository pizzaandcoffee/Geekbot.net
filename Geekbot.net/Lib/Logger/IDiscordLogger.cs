using System.Threading.Tasks;
using Discord;

namespace Geekbot.net.Lib.Logger
{
    public interface IDiscordLogger
    {
        Task Log(LogMessage message);
    }
}