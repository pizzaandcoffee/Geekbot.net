using System;

namespace Geekbot.net.Lib.DiceParser
{
    public class DieResult
    {
        // public int Result { get; set; }
        public int Roll1 { get; set; }
        public int Roll2 { get; set; }
        public DieAdvantageType AdvantageType { get; set; }

        public override string ToString()
        {
            return AdvantageType switch
            {
                DieAdvantageType.Advantage => Roll1 > Roll2 ? $"(**{Roll1}**, {Roll2})" : $"({Roll1}, **{Roll2}**)",
                DieAdvantageType.Disadvantage => Roll1 < Roll2 ? $"(**{Roll1}**, {Roll2})" : $"({Roll1}, **{Roll2}**)",
                _ => Result.ToString()
            };
        }

        public int Result => AdvantageType switch
        {
            DieAdvantageType.None => Roll1,
            DieAdvantageType.Advantage => Math.Max(Roll1, Roll2),
            DieAdvantageType.Disadvantage => Math.Min(Roll1, Roll2),
            _ => 0
        };
    }
}