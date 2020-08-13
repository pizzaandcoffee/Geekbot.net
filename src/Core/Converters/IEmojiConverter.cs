namespace Geekbot.Core.Converters
{
    public interface IEmojiConverter
    {
        string NumberToEmoji(int number);
        string TextToEmoji(string text);
    }
}