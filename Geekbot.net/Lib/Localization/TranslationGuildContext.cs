using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geekbot.net.Lib.Localization
{
    public class TranslationGuildContext
    {
        public ITranslationHandler TranslationHandler { get; }
        public string Language { get; }
        public Dictionary<string, List<string>> Dict { get; }
        
        public TranslationGuildContext(ITranslationHandler translationHandler, string language, Dictionary<string, List<string>> dict)
        {
            TranslationHandler = translationHandler;
            Language = language;
            Dict = dict;
        }

        public string GetString(string stringToFormat, params object[] args)
        {
            return string.Format(Dict[stringToFormat].First() ?? "", args);
        }
        
        public string FormatDateTimeAsRemaining(DateTimeOffset dateTime)
        {
            var remaining = dateTime - DateTimeOffset.Now;
            const string formattable = "{0} {1}";
            var sb = new StringBuilder();
            
            if (remaining.Days > 0)
            {
                var s = GetTimeString(TimeTypes.Days);
                sb.AppendFormat(formattable, remaining.Days, GetSingOrPlur(remaining.Days, s));
            }
            
            if (remaining.Hours > 0)
            {
                if (sb.Length > 0) sb.Append(", ");
                var s = GetTimeString(TimeTypes.Hours);
                sb.AppendFormat(formattable, remaining.Hours, GetSingOrPlur(remaining.Hours, s));
            }
            
            if (remaining.Minutes > 0)
            {
                if (sb.Length > 0) sb.Append(", ");
                var s = GetTimeString(TimeTypes.Minutes);
                sb.AppendFormat(formattable, remaining.Minutes, GetSingOrPlur(remaining.Minutes, s));
            }
            
            if (remaining.Seconds > 0)
            {
                if (sb.Length > 0)
                {
                    var and = TranslationHandler.GetStrings(Language, "dateTime", "And").First();
                    sb.AppendFormat(" {0} ", and);
                }
                var s = GetTimeString(TimeTypes.Seconds);
                sb.AppendFormat(formattable, remaining.Seconds, GetSingOrPlur(remaining.Seconds, s));
            }
            
            return sb.ToString().Trim();
        }
        
        public Task<bool> SetLanguage(ulong guildId, string language)
        {
            return TranslationHandler.SetLanguage(guildId, language);
        }

        private List<string> GetTimeString(TimeTypes type)
        {
            return TranslationHandler.GetStrings(Language, "dateTime", type.ToString());
        }

        private string GetSingOrPlur(int number, List<string> versions)
        {
            return number == 1 ? versions[0] : versions[1];
        }

        private enum TimeTypes
        {
            Days,
            Hours,
            Minutes,
            Seconds
        }
    }
}