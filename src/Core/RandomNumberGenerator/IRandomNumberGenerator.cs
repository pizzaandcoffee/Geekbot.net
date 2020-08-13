namespace Geekbot.Core.RandomNumberGenerator
{
    public interface IRandomNumberGenerator
    {
        int Next(int minValue, int maxExclusiveValue);
    }
}