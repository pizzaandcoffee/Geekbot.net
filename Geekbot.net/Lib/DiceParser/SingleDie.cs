using System.Collections.Generic;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.RandomNumberGenerator;

namespace Geekbot.net.Lib.DiceParser
{
    public class SingleDie
    {
        private readonly IRandomNumberGenerator _random;

        public SingleDie(IRandomNumberGenerator random)
        {
            _random = random;
        }

        public int Sides { get; set; } = 20;
        public int Amount { get; set; } = 1;
        public DieAdvantageType AdvantageType { get; set; } = DieAdvantageType.None;

        public string DiceName => AdvantageType switch
        {
            DieAdvantageType.Advantage => $"{Amount}d{Sides} (with advantage)",
            DieAdvantageType.Disadvantage => $"{Amount}d{Sides} (with disadvantage)",
            _ => $"{Amount}d{Sides}"
        };

        public List<DieResult> Roll()
        {
            var results = new List<DieResult>();

            Amount.Times(() =>
            {
                var result = new DieResult
                {
                    Roll1 = _random.Next(1, Sides + 1),
                    AdvantageType = AdvantageType
                };

                if (AdvantageType == DieAdvantageType.Advantage || AdvantageType == DieAdvantageType.Disadvantage)
                {
                    result.Roll2 = _random.Next(1, Sides);
                }

                results.Add(result);
            });

            return results;
        }

        public void ValidateDie()
        {
            if (Amount < 1)
            {
                throw new DiceException("To few dice, must be a minimum of 1");
            }
            if (Amount > 24)
            {
                throw new DiceException("To many dice, maximum allowed is 24") { DiceName = DiceName };
            }

            if (Sides < 2)
            {
                throw new DiceException("Die must have at least 2 sides") { DiceName = DiceName };
            }
            
            if (Sides > 144)
            {
                throw new DiceException("Die can not have more than 144 sides") { DiceName = DiceName };
            }
        }
    }
}