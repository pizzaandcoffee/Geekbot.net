using System.Collections.Generic;

namespace Geekbot.net.Lib.Highscores
{
    public interface IHighscoreManager
    {
        Dictionary<HighscoreUserDto, int> GetHighscoresWithUserData(HighscoreTypes type, ulong guildId, int amount);
        Dictionary<ulong, int> GetMessageList(ulong guildId, int amount);
        Dictionary<ulong, int> GetKarmaList(ulong guildId, int amount);
        Dictionary<ulong, int> GetRollsList(ulong guildId, int amount);
    }
}