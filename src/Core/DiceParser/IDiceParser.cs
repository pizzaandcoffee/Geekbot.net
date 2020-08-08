namespace Geekbot.Core.DiceParser
{
    public interface IDiceParser
    {
        DiceInput Parse(string input);
    }
}