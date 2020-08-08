using System.Collections;
using System.Text;

namespace Geekbot.Core.Converters
{
    public class EmojiConverter : IEmojiConverter
    {
        public string NumberToEmoji(int number)
        {
            if (number == 10)
            {
                return "🔟";
            }
            var emojiMap = new[]
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
            var numbers = number.ToString().ToCharArray();
            var returnString = new StringBuilder();
            foreach (var n in numbers)
            {
                returnString.Append(emojiMap[int.Parse(n.ToString())]);
            }
            return returnString.ToString();
        }

        public string TextToEmoji(string text)
        {
            var emojiMap = new Hashtable
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
            var letters = text.ToUpper().ToCharArray();
            var returnString = new StringBuilder();
            foreach (var n in letters)
            {
                var emoji = emojiMap[n] ?? n;
                returnString.Append(emoji);
            }
            return returnString.ToString();
        }
    }
}