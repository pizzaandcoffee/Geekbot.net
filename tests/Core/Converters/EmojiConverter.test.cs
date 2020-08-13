using Geekbot.Core.Converters;
using Xunit;

namespace Tests.Core.Converters
{
    public class EmojiConverterTest
    {
        public class NumberToEmojiTestDto
        {
            public int Number { get; set; }
            public string Expected { get; set; }
        }
        
        public static TestData<NumberToEmojiTestDto> NumberToEmojiTestData =>
            new TestData<NumberToEmojiTestDto>
            {
                {
                    "2",
                    new NumberToEmojiTestDto
                    {
                        Number = 2,
                        Expected = ":two:"
                    }
                },
                {
                    "10",
                    new NumberToEmojiTestDto
                    {
                        Number = 10,
                        Expected = "🔟"
                    }
                },
                {
                    "15",
                    new NumberToEmojiTestDto
                    {
                        Number = 15,
                        Expected = ":one::five:"
                    }
                },
                {
                    "null",
                    new NumberToEmojiTestDto
                    {
                        Number = 0,
                        Expected = ":zero:"
                    }
                }
            };

        [Theory, MemberData(nameof(NumberToEmojiTestData))]
        public void NumberToEmoji(string testName, NumberToEmojiTestDto testData)
        {
            var emojiConverter = new EmojiConverter();
            var result = emojiConverter.NumberToEmoji(testData.Number);
            Assert.Equal(result, testData.Expected);
        }
        
        public class TextToEmojiTestDto
        {
            public string Text { get; set; }
            public string Expected { get; set; }
        }

        public static TestData<TextToEmojiTestDto> TextToEmojiTestData =>
            new TestData<TextToEmojiTestDto>
            {
                {
                    "Test",
                    new TextToEmojiTestDto
                    {
                        Text = "test",
                        Expected = ":regional_indicator_t: :regional_indicator_e: :regional_indicator_s: :regional_indicator_t: "
                    }
                },
                {
                    "Best3+?",
                    new TextToEmojiTestDto
                    {
                        Text = "Best3+?",
                        Expected = ":b: :regional_indicator_e: :regional_indicator_s: :regional_indicator_t: :three: :heavy_plus_sign: :question: "
                    }
                }
            };

        [Theory, MemberData(nameof(TextToEmojiTestData))]
        public void TextToEmoji(string testName, TextToEmojiTestDto testData)
        {
            var emojiConverter = new EmojiConverter();
            var result = emojiConverter.TextToEmoji(testData.Text);
            Assert.Equal(result, testData.Expected);
        }
    }
}