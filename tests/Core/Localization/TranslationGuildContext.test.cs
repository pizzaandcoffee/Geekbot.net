using System;
using System.Collections.Generic;
using Geekbot.Core.Localization;
using Moq;
using Xunit;

namespace Tests.Core.Localization
{
    public class TranslationGuildContext_test
    {
        public class FormatDateTimeAsRemainingTestDto
        {
            public DateTimeOffset DateTime { get; set; }
            public string Expected { get; set; }
        }

        public static TestData<FormatDateTimeAsRemainingTestDto> FormatDateTimeAsRemainingData =>
            new TestData<FormatDateTimeAsRemainingTestDto>
            {
                {
                    "Wait for days",
                    new FormatDateTimeAsRemainingTestDto
                    {
                        DateTime = DateTimeOffset.Now.AddDays(5),
                        Expected = "4 days, 23 hours, 59 minutes and 59 seconds"
                    }
                },
                {
                    "Wait for minutes",
                    new FormatDateTimeAsRemainingTestDto
                    {
                        DateTime = DateTimeOffset.Now.AddMinutes(5),
                        Expected = "4 minutes and 59 seconds"
                    }
                },
                {
                    "Wait for seconds",
                    new FormatDateTimeAsRemainingTestDto
                    {
                        DateTime = DateTimeOffset.Now.AddSeconds(5),
                        Expected = "4 seconds"
                    }
                }
            };
        
        [Theory, MemberData(nameof(FormatDateTimeAsRemainingData))]
        public void FormatDateTimeAsRemaining(string testName, FormatDateTimeAsRemainingTestDto testData)
        {
            var translationHandlerMock = new Mock<ITranslationHandler>(MockBehavior.Loose);
            translationHandlerMock
                .Setup(thm => thm.GetString("EN", "dateTime", "Days"))
                .Returns("day|days");
            translationHandlerMock
                .Setup(thm => thm.GetString("EN", "dateTime", "Hours"))
                .Returns("hour|hours");
            translationHandlerMock
                .Setup(thm => thm.GetString("EN", "dateTime", "Minutes"))
                .Returns("minute|minutes");
            translationHandlerMock
                .Setup(thm => thm.GetString("EN", "dateTime", "Seconds"))
                .Returns("second|seconds");
            translationHandlerMock
                .Setup(thm => thm.GetString("EN", "dateTime", "And"))
                .Returns("and");
            
            var context = new TranslationGuildContext(translationHandlerMock.Object, "EN", new Dictionary<string, string>());
            var result = context.FormatDateTimeAsRemaining(testData.DateTime);
            Assert.Equal(result, testData.Expected);
        }
    }
}