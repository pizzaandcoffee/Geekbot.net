using System.Collections.Generic;
using Geekbot.net.Lib.Levels;
using Xunit;

namespace Tests.Lib.Levels
{
    public class LevelCalcTest
    {
        public class LevelCalcTestDto
        {
            public int Messages { get; set; }
            public int ExpectedLevel { get; set; }
        }

        public static TestData<LevelCalcTestDto> LevelCalcTestData =>
            new TestData<LevelCalcTestDto>()
            {
                {
                    "500",
                    new LevelCalcTestDto
                    {
                        Messages = 500,
                        ExpectedLevel = 13
                    }
                },
                {
                    "41659",
                    new LevelCalcTestDto
                    {
                        Messages = 41659,
                        ExpectedLevel = 55
                    }
                },
                {
                    "0",
                    new LevelCalcTestDto
                    {
                        Messages = 0,
                        ExpectedLevel = 1
                    }
                },
                {
                    "4000000",
                    new LevelCalcTestDto
                    {
                        Messages = 4000000,
                        ExpectedLevel = 101
                    }
                }
            };

        [Theory, MemberData(nameof(LevelCalcTestData))]
        public void GetLevel(string testName, LevelCalcTestDto testData)
        {
            var levelCalc = new LevelCalc();
            var result = levelCalc.GetLevel(testData.Messages);
            Assert.Equal(result, testData.ExpectedLevel);
        }
    }
}