using System;
using System.Linq;
using System.Text.RegularExpressions;
using Geekbot.Core.RandomNumberGenerator;

namespace Geekbot.Core.DiceParser
{
    public class DiceParser : IDiceParser
    {
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly Regex _inputRegex;
        private readonly Regex _singleDieRegex;

        public DiceParser(IRandomNumberGenerator randomNumberGenerator)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _inputRegex = new Regex(
                @"((?<DieAdvantage>\+\d+d\d+)|(?<DieDisadvantage>\-\d+d\d+)|(?<DieNormal>\d+d\d+)|(?<Keywords>(total))|(?<SkillModifer>(\+|\-)\d+))\s",
                RegexOptions.Compiled | RegexOptions.IgnoreCase,
                new TimeSpan(0, 0, 2));
            _singleDieRegex = new Regex(
                @"\d+d\d+",
                RegexOptions.Compiled | RegexOptions.IgnoreCase,
                new TimeSpan(0, 0, 0, 0, 500));
        }

        public DiceInput Parse(string input)
        {
            // adding a whitespace at the end, otherwise the parser might pickup on false items
            var inputWithExtraWhitespace = $"{input} ";
            
            var matches = _inputRegex.Matches(inputWithExtraWhitespace);
            var result = new DiceInput();
            var resultOptions = new DiceInputOptions();

            foreach (Match match in matches)
            {
                foreach (Group matchGroup in match.Groups)
                {
                    if (matchGroup.Success)
                    {
                        switch (matchGroup.Name)
                        {
                            case "DieNormal":
                                result.Dice.Add(Die(matchGroup.Value, DieAdvantageType.None));
                                break;
                            case "DieAdvantage":
                                result.Dice.Add(Die(matchGroup.Value, DieAdvantageType.Advantage));
                                break;
                            case "DieDisadvantage":
                                result.Dice.Add(Die(matchGroup.Value, DieAdvantageType.Disadvantage));
                                break;
                            case "Keywords":
                                Keywords(matchGroup.Value, ref resultOptions);
                                break;
                            case "SkillModifer":
                                result.SkillModifier = SkillModifer(matchGroup.Value);
                                break;
                        }
                    }
                }
            }

            if (!result.Dice.Any())
            {
                result.Dice.Add(new SingleDie(_randomNumberGenerator));
            }

            result.Options = resultOptions;

            return result;
        }

        private SingleDie Die(string match, DieAdvantageType advantageType)
        {
            var x = _singleDieRegex.Match(match).Value.Split('d');
            var die = new SingleDie(_randomNumberGenerator)
            {
                Amount = int.Parse(x[0]),
                Sides = int.Parse(x[1]),
                AdvantageType = advantageType
            };
            die.ValidateDie();
            return die;
        }

        private int SkillModifer(string match)
        {
            return int.Parse(match);
        }

        private void Keywords(string match, ref DiceInputOptions options)
        {
            switch (match)
            {
                case "total":
                    options.ShowTotal = true;
                    break;
            }
        }
    }
}