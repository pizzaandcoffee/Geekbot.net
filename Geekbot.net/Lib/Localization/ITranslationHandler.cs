using System.Collections.Generic;
using Discord.Commands;

namespace Geekbot.net.Lib.Localization
{
    public interface ITranslationHandler
    {
        string GetString(ulong guildId, string command, string stringName);
        Dictionary<string, string> GetDict(ICommandContext context);
        Dictionary<string, string> GetDict(ICommandContext context, string command);
        bool SetLanguage(ulong guildId, string language);
        List<string> GetSupportedLanguages();
    }
}