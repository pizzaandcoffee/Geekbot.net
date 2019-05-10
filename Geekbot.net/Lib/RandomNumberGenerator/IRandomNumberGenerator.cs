namespace Geekbot.net.Lib.RandomNumberGenerator
{
    public interface IRandomNumberGenerator
    {
        int Next(int minValue, int maxExclusiveValue);
    }
}