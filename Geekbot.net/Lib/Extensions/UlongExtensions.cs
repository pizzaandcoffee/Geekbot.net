using System;

namespace Geekbot.net.Lib.Extensions
{
    public static class UlongExtensions
    {
        public static long AsLong(this ulong thing)
        {
            return Convert.ToInt64(thing);
        }
    }
}