using System;
using System.Text;

namespace Geekbot.Bot.Utils
{
    public class DateLocalization
    {
        public static string FormatDateTimeAsRemaining(DateTimeOffset dateTime)
        {
            var remaining = dateTime - DateTimeOffset.Now;
            const string formattable = "{0} {1}";
            var sb = new StringBuilder();
            
            if (remaining.Days > 0)
            {
                sb.AppendFormat(formattable, remaining.Days, GetSingularOrPlural(remaining.Days, Localization.Internal.Days));
            }
            
            if (remaining.Hours > 0)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.AppendFormat(formattable, remaining.Hours, GetSingularOrPlural(remaining.Hours, Localization.Internal.Hours));
            }
            
            if (remaining.Minutes > 0)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.AppendFormat(formattable, remaining.Minutes, GetSingularOrPlural(remaining.Minutes, Localization.Internal.Minutes));
            }
            
            if (remaining.Seconds > 0)
            {
                if (sb.Length > 0)
                {
                    sb.AppendFormat(" {0} ", Localization.Internal.And);
                }
                sb.AppendFormat(formattable, remaining.Seconds, GetSingularOrPlural(remaining.Seconds, Localization.Internal.Seconds));
            }
            
            return sb.ToString().Trim();
        }
        
        private static string GetSingularOrPlural(int number, string rawString)
        {
            var versions = rawString.Split('|');
            return number == 1 ? versions[0] : versions[1];
        }
    }
}