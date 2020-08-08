using System.Threading.Tasks;
using Geekbot.Core.Database.Models;

namespace Geekbot.Core.GuildSettingsManager
{
    public interface IGuildSettingsManager
    {
        GuildSettingsModel GetSettings(ulong guildId, bool createIfNonExist = true);
        Task UpdateSettings(GuildSettingsModel settings);
    }
}