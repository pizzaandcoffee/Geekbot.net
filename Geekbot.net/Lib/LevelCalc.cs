using System;

namespace Geekbot.net.Lib
{
    internal class LevelCalc
    {
        private static int GetExperienceAtLevel(int level)
        {
            double total = 0;
            for (var i = 1; i < level; i++)
                total += Math.Floor(i + 300 * Math.Pow(2, i / 7.0));

            return (int) Math.Floor(total / 16);
        }

        public static int GetLevelAtExperience(int experience)
        {
            int index;

            for (index = 0; index < 120; index++)
                if (GetExperienceAtLevel(index + 1) > experience)
                    break;

            return index;
        }
    }
}