using System.Threading.Tasks;
using Geekbot.net.Database.Models;

namespace Geekbot.net.Lib.GuildSettingsManager
{
    public interface IGuildSettingsManager
    {
        GuildSettingsModel GetSettings(ulong guildId, bool createIfNonExist = true);
        Task UpdateSettings(GuildSettingsModel settings);
    }
}