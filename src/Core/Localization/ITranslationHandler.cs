using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.Core.Localization
{
    public interface ITranslationHandler
    {
        Task<string> GetString(ulong guildId, string command, string stringName);
        string GetString(string language, string command, string stringName);
        Task<Dictionary<string, string>> GetDict(ICommandContext context, string command);
        Task<TranslationGuildContext> GetGuildContext(ICommandContext context);
        Task<bool> SetLanguage(ulong guildId, string language);
        List<string> SupportedLanguages { get; }
    }
}