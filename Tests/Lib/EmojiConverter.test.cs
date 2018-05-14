using System.Collections.Generic;
using Geekbot.net.Lib.Converters;
using Xunit;

namespace Tests.Lib
{
    public class EmojiConverterTest
    {
        public static IEnumerable<object[]> NumberToEmojiTestData
        {
            get
            {
                yield return new object[]
                {
                    2,
                    ":two:"
                };
                
                yield return new object[]
                {
                    10,
                    "🔟"
                };
                
                yield return new object[]
                {
                    15,
                    ":one::five:"
                };
                
                yield return new object[]
                {
                    null,
                    ":zero:"
                };
            }
        }


        [Theory, MemberData(nameof(NumberToEmojiTestData))]
        public void NumberToEmoji(int number, string expectedResult)
        {
            var emojiConverter = new EmojiConverter();
            var result = emojiConverter.NumberToEmoji(number);
            Assert.Equal(result, expectedResult);
        }
        
        public static IEnumerable<object[]> TextToEmojiTestData
        {
            get
            {
                yield return new object[]
                {
                    "test",
                    ":regional_indicator_t: :regional_indicator_e: :regional_indicator_s: :regional_indicator_t: "
                };
                yield return new object[]
                {
                    "Best3+?",
                    ":b: :regional_indicator_e: :regional_indicator_s: :regional_indicator_t: :three: :heavy_plus_sign: :question: "
                };
            }
        }


        [Theory, MemberData(nameof(TextToEmojiTestData))]
        public void TextToEmoji(string text, string expectedResult)
        {
            var emojiConverter = new EmojiConverter();
            var result = emojiConverter.TextToEmoji(text);
            Assert.Equal(result, expectedResult);
        }
    }
}