namespace Geekbot.net.Lib.Converters
{
    public interface IEmojiConverter
    {
        string NumberToEmoji(int number);
        string TextToEmoji(string text);
    }
}