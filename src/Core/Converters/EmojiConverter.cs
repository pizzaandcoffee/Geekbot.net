using System.Collections;
using System.Text;

namespace Geekbot.Core.Converters
{
    public static class EmojiConverter
    {
        private static readonly string[] NumberEmojiMap =
        {
            ":zero:",
            ":one:",
            ":two:",
            ":three:",
            ":four:",
            ":five:",
            ":six:",
            ":seven:",
            ":eight:",
            ":nine:"
        };

        public static string NumberToEmoji(int number)
        {
            if (number == 10)
            {
                return "🔟";
            }

            var numbers = number.ToString().ToCharArray();
            var returnString = new StringBuilder();
            foreach (var n in numbers)
            {
                returnString.Append(NumberEmojiMap[int.Parse(n.ToString())]);
            }
            return returnString.ToString();
        }

        private static readonly Hashtable TextEmojiMap = new Hashtable
        {
            ['A'] = ":regional_indicator_a: ",
            ['B'] = ":b: ",
            ['C'] = ":regional_indicator_c: ",
            ['D'] = ":regional_indicator_d: ",
            ['E'] = ":regional_indicator_e: ",
            ['F'] = ":regional_indicator_f: ",
            ['G'] = ":regional_indicator_g: ",
            ['H'] = ":regional_indicator_h: ",
            ['I'] = ":regional_indicator_i: ",
            ['J'] = ":regional_indicator_j: ",
            ['K'] = ":regional_indicator_k: ",
            ['L'] = ":regional_indicator_l: ",
            ['M'] = ":regional_indicator_m: ",
            ['N'] = ":regional_indicator_n: ",
            ['O'] = ":regional_indicator_o: ",
            ['P'] = ":regional_indicator_p: ",
            ['Q'] = ":regional_indicator_q: ",
            ['R'] = ":regional_indicator_r: ",
            ['S'] = ":regional_indicator_s: ",
            ['T'] = ":regional_indicator_t: ",
            ['U'] = ":regional_indicator_u: ",
            ['V'] = ":regional_indicator_v: ",
            ['W'] = ":regional_indicator_w: ",
            ['X'] = ":regional_indicator_x: ",
            ['Y'] = ":regional_indicator_y: ",
            ['Z'] = ":regional_indicator_z: ",
            ['!'] = ":exclamation: ",
            ['?'] = ":question: ",
            ['#'] = ":hash: ",
            ['*'] = ":star2: ",
            ['+'] = ":heavy_plus_sign: ",
            ['0'] = ":zero: ",
            ['1'] = ":one: ",
            ['2'] = ":two: ",
            ['3'] = ":three: ",
            ['4'] = ":four: ",
            ['5'] = ":five: ",
            ['6'] = ":six: ",
            ['7'] = ":seven: ",
            ['8'] = ":eight: ",
            ['9'] = ":nine: ",
            [' '] = " "
        };
        
        public static string TextToEmoji(string text)
        {
            var letters = text.ToUpper().ToCharArray();
            var returnString = new StringBuilder();
            foreach (var n in letters)
            {
                var emoji = TextEmojiMap[n] ?? n;
                returnString.Append(emoji);
            }
            return returnString.ToString();
        }

        private static readonly Hashtable RegionalIndicatorMap = new Hashtable()
        {
            ['A'] = new Rune(0x1F1E6),
            ['B'] = new Rune(0x1F1E7),
            ['C'] = new Rune(0x1F1E8),
            ['D'] = new Rune(0x1F1E9),
            ['E'] = new Rune(0x1F1EA),
            ['F'] = new Rune(0x1F1EB),
            ['G'] = new Rune(0x1F1EC),
            ['H'] = new Rune(0x1F1ED),
            ['I'] = new Rune(0x1F1EE),
            ['J'] = new Rune(0x1F1EF),
            ['K'] = new Rune(0x1F1F0),
            ['L'] = new Rune(0x1F1F1),
            ['M'] = new Rune(0x1F1F2),
            ['N'] = new Rune(0x1F1F3),
            ['O'] = new Rune(0x1F1F4),
            ['P'] = new Rune(0x1F1F5),
            ['Q'] = new Rune(0x1F1F6),
            ['R'] = new Rune(0x1F1F7),
            ['S'] = new Rune(0x1F1F8),
            ['T'] = new Rune(0x1F1F9),
            ['U'] = new Rune(0x1F1FA),
            ['V'] = new Rune(0x1F1FB),
            ['W'] = new Rune(0x1F1FC),
            ['X'] = new Rune(0x1F1FD),
            ['Y'] = new Rune(0x1F1FE),
            ['Z'] = new Rune(0x1F1FF)
        };
        
        public static string CountryCodeToEmoji(string countryCode)
        {
            var letters = countryCode.ToUpper().ToCharArray();
            var returnString = new StringBuilder();
            foreach (var n in letters)
            {
                var emoji = RegionalIndicatorMap[n];
                returnString.Append(emoji);
            }
            return returnString.ToString();
        }
    }
}