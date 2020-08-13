using System;

namespace Geekbot.Core.Extensions
{
    public static class UlongExtensions
    {
        public static long AsLong(this ulong thing)
        {
            return Convert.ToInt64(thing);
        }
    }
}