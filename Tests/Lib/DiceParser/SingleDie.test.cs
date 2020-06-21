using Geekbot.net.Lib.DiceParser;
using Geekbot.net.Lib.RandomNumberGenerator;
using Xunit;

namespace Tests.Lib.DiceParser
{
    public class SingleDieTest
    {
        public struct SingleDieNameTestDto
        {
            public DieAdvantageType AdvantageType { get; set; }
            public string Expected { get; set; }
        }

        public static TestData<SingleDieNameTestDto> SingleDieNameTestData => new TestData<SingleDieNameTestDto>
        {
            {
                "No advantage",
                new SingleDieNameTestDto()
                {
                    AdvantageType = DieAdvantageType.None,
                    Expected = "1d20"
                }
            },
            {
                "With advantage",
                new SingleDieNameTestDto()
                {
                    AdvantageType = DieAdvantageType.Advantage,
                    Expected = "1d20 (with advantage)"
                }
            },
            {
                "With disadvantage",
                new SingleDieNameTestDto()
                {
                    AdvantageType = DieAdvantageType.Disadvantage,
                    Expected = "1d20 (with disadvantage)"
                }
            }
        };

        [Theory, MemberData(nameof(SingleDieNameTestData))]
        public void SingleDieNameTestFunc(string testName, SingleDieNameTestDto testData)
        {
            var die = new SingleDie(new RandomNumberGenerator()) {AdvantageType = testData.AdvantageType};
            Assert.Equal(die.DiceName, testData.Expected);
        }

        public struct SingleDieValidationTestDto
        {
            public int Sides { get; set; }
            public int Amount { get; set; }
            public bool PassValidation { get; set; }
        }

        public static TestData<SingleDieValidationTestDto> SingleDieValidationTestData => new TestData<SingleDieValidationTestDto>
        {
            {
                "To many sides",
                new SingleDieValidationTestDto()
                {
                    Amount = 1,
                    Sides = 200,
                    PassValidation = false
                }
            },
            {
                "To few sides",
                new SingleDieValidationTestDto()
                {
                    Amount = 1,
                    Sides = 1,
                    PassValidation = false
                }
            },
            {
                "To many Dice",
                new SingleDieValidationTestDto()
                {
                    Amount = 200,
                    Sides = 20,
                    PassValidation = false
                }
            },
            {
                "To few Dice",
                new SingleDieValidationTestDto()
                {
                    Amount = 0,
                    Sides = 20,
                    PassValidation = false
                }
            },
            {
                "Correct Dice",
                new SingleDieValidationTestDto()
                {
                    Amount = 1,
                    Sides = 20,
                    PassValidation = true
                }
            }
        };

        [Theory, MemberData(nameof(SingleDieValidationTestData))]
        public void SingleDieValidationTestFunc(string testName, SingleDieValidationTestDto testData)
        {
            var die = new SingleDie(new RandomNumberGenerator())
            {
                Amount = testData.Amount,
                Sides = testData.Sides
            };

            if (testData.PassValidation)
            {
                die.ValidateDie();
            }
            else
            {
                Assert.Throws<DiceException>(() => die.ValidateDie());                
            }
        }
    }
}