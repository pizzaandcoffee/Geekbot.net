using System.Collections.Generic;
using System.Threading.Tasks;
using Geekbot.net.Lib;
using Xunit;

namespace Tests.Lib
{
    public class LevelCalc_test
    {
        public static IEnumerable<object[]> LevelCalcTestData
        {
            get
            {
                yield return new object[]
                {
                    500,
                    13
                };
                
                yield return new object[]
                {
                    41659,
                    55
                };
                
                yield return new object[]
                {
                    0,
                    1
                };
                
                yield return new object[]
                {
                    4000000,
                    101
                };
            }
        }


        [Theory, MemberData(nameof(LevelCalcTestData))]
        public async Task GetLevel(int messages, int expectedResult)
        {
            var levelCalc = new LevelCalc();
            var result = levelCalc.GetLevel(messages);
            Assert.Equal(result, expectedResult);
        }
    }
}