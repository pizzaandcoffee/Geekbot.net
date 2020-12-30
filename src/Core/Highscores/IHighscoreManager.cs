using System.Collections.Generic;

namespace Geekbot.Core.Highscores
{
    public interface IHighscoreManager
    {
        Dictionary<HighscoreUserDto, int> GetHighscoresWithUserData(HighscoreTypes type, ulong guildId, int amount, string season = null);
        Dictionary<ulong, int> GetMessageList(ulong guildId, int amount);
        Dictionary<ulong, int> GetMessageSeasonList(ulong guildId, int amount, string season);
        Dictionary<ulong, int> GetKarmaList(ulong guildId, int amount);
        Dictionary<ulong, int> GetRollsList(ulong guildId, int amount);
    }
}