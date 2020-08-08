using System;

namespace Geekbot.Core.Extensions
{
    public static class LongExtensions
    {
        public static ulong AsUlong(this long thing)
        {
            return Convert.ToUInt64(thing);
        }
    }
}