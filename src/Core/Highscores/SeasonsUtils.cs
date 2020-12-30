using System;
using System.Globalization;

namespace Geekbot.Core.Highscores
{
    public class SeasonsUtils
    {
        public static string GetCurrentSeason()
        {
            var now = DateTime.Now;
            var year = (now.Year - 2000).ToString(CultureInfo.InvariantCulture);
            var quarter = Math.Ceiling(now.Month / 3.0).ToString(CultureInfo.InvariantCulture);
            return $"{year}Q{quarter}";
        }
    }
}