using System.Threading.Tasks;
using Discord;

namespace Geekbot.Core.Logger
{
    public interface IDiscordLogger
    {
        Task Log(LogMessage message);
    }
}