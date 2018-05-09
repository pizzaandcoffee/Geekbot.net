using System;

namespace Geekbot.net.Lib.Extensions
{
    public static class LongExtensions
    {
        public static ulong AsUlong(this long thing)
        {
            return Convert.ToUInt64(thing);
        }
    }
}