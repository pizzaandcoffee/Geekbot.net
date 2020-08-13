using System.Linq;

namespace Geekbot.Core.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeFirst(this string source)
        {
            return source.First().ToString().ToUpper() + source.Substring(1);
        }
    }
}