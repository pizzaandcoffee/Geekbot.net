using System.Collections.Generic;
using System.Text.Json;
using Geekbot.Core.DiceParser;
using Geekbot.Core.GlobalSettings;
using Geekbot.Core.RandomNumberGenerator;
using Moq;
using Xunit;

namespace Tests.Core.DiceParser
{
    public class DiceParserTest
    {
        private static readonly RandomNumberGenerator _randomNumberGenerator = new RandomNumberGenerator(new Mock<IGlobalSettings>().Object);

        public struct DiceParserTestDto
        {
            public string Input { get; set; }
            public DiceInput Expected { get; set; }
        }
        
        public static TestData<DiceParserTestDto> DiceParserTestData =>
            new TestData<DiceParserTestDto>
            {
                {
                    "Empty Input",
                    new DiceParserTestDto
                    {
                        Input = "",
                        Expected = new DiceInput()
                        {
                            Dice = new List<SingleDie>
                            {
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 1,
                                    Sides = 20,
                                    AdvantageType = DieAdvantageType.None
                                }
                            }
                        }
                    }
                },
                {
                    "Simple 1d20 input",
                    new DiceParserTestDto
                    {
                        Input = "1d20",
                        Expected = new DiceInput()
                        {
                            Dice = new List<SingleDie>
                            {
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 1,
                                    Sides = 20,
                                    AdvantageType = DieAdvantageType.None
                                }
                            }
                        }
                    }
                },
                {
                    "2d8 with advantage",
                    new DiceParserTestDto
                    {
                        Input = "+2d8",
                        Expected = new DiceInput()
                        {
                            Dice = new List<SingleDie>
                            {
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 2,
                                    Sides = 8,
                                    AdvantageType = DieAdvantageType.Advantage
                                }
                            }
                        }
                    }
                },
                {
                    "2d8 with disadvantage",
                    new DiceParserTestDto
                    {
                        Input = "-2d8",
                        Expected = new DiceInput()
                        {
                            Dice = new List<SingleDie>
                            {
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 2,
                                    Sides = 8,
                                    AdvantageType = DieAdvantageType.Disadvantage
                                }
                            }
                        }
                    }
                },
                {
                    "multiple dice",
                    new DiceParserTestDto
                    {
                        Input = "2d8 2d6",
                        Expected = new DiceInput()
                        {
                            Dice = new List<SingleDie>
                            {
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 2,
                                    Sides = 8
                                },
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 2,
                                    Sides = 6
                                }
                            }
                        }
                    }
                },
                {
                    "with skill modifier, no dice",
                    new DiceParserTestDto
                    {
                        Input = "+2",
                        Expected = new DiceInput()
                        {
                            Dice = new List<SingleDie>
                            {
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 1,
                                    Sides = 20
                                }
                            },
                            SkillModifier = 2
                        }
                    }
                },
                {
                    "8d6 with total",
                    new DiceParserTestDto
                    {
                        Input = "8d6 total",
                        Expected = new DiceInput()
                        {
                            Dice = new List<SingleDie>
                            {
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 8,
                                    Sides = 6
                                }
                            },
                            Options = new DiceInputOptions
                            {
                                ShowTotal = true
                            } 
                        }
                    }
                },
                {
                    "All posibilities",
                    new DiceParserTestDto
                    {
                        Input = "2d20 +1d20 -1d20 +1 total",
                        Expected = new DiceInput()
                        {
                            Dice = new List<SingleDie>
                            {
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 2,
                                    Sides = 20
                                },
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 1,
                                    Sides = 20,
                                    AdvantageType = DieAdvantageType.Advantage
                                },
                                new SingleDie(_randomNumberGenerator)
                                {
                                    Amount = 1,
                                    Sides = 20,
                                    AdvantageType = DieAdvantageType.Disadvantage
                                },
                            },
                            Options = new DiceInputOptions
                            {
                                ShowTotal = true
                            },
                            SkillModifier = 1
                        }
                    }
                },
            };

        [Theory, MemberData(nameof(DiceParserTestData))]
        public void DiceParserTestFunc(string testName, DiceParserTestDto testData)
        {
            var parser = new Geekbot.Core.DiceParser.DiceParser(_randomNumberGenerator);
            var result = parser.Parse(testData.Input);
            
            Assert.Equal(JsonSerializer.Serialize(result), JsonSerializer.Serialize(testData.Expected));
        }
    }
}