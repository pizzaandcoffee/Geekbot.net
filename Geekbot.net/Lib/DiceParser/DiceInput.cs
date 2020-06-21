using System.Collections.Generic;

namespace Geekbot.net.Lib.DiceParser
{
    public class DiceInput
    {
        public List<SingleDie> Dice { get; set; } = new List<SingleDie>();
        public DiceInputOptions Options { get; set; } = new DiceInputOptions();
        public int SkillModifier { get; set; }
    }
}