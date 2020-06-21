using System;

namespace Geekbot.net.Lib.Extensions
{
    public static class IntExtensions
    {
        public static void Times(this int count, Action action)
        {
            for (var i = 0; i < count; i++)
            {
                action();
            }
        }
    }
}