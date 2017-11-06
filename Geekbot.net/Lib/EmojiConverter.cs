using System.Text;

namespace Geekbot.net.Lib
{
    public class EmojiConverter : IEmojiConverter
    {
        public string numberToEmoji(int number)
        {
            if (number == 10)
            {
                return "🔟";
            }
            var emojiMap = new string[] {"0⃣", "1⃣", "2⃣", "3⃣", "4⃣", "5⃣", "6⃣", "7⃣", "8⃣", "9⃣"};
            var numbers = number.ToString().ToCharArray();
            var returnString = new StringBuilder();
            foreach (var n in numbers)
            {
                returnString.Append(emojiMap[int.Parse(n.ToString())]);
            }
            return returnString.ToString();
        }
    }
    
    public interface IEmojiConverter
    {
        string numberToEmoji(int number);
    }
}