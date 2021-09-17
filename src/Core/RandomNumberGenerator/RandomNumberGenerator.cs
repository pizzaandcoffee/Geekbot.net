using System;

namespace Geekbot.Core.RandomNumberGenerator
{
    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly System.Security.Cryptography.RandomNumberGenerator rng;

        public RandomNumberGenerator()
        {
            rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        }

        public int Next(int minValue, int maxInclusiveValue)
        {
            if (minValue == maxInclusiveValue)
            {
                return maxInclusiveValue;
            }
            
            if (minValue >= maxInclusiveValue)
            {
                throw new ArgumentOutOfRangeException("minValue", "must be lower than maxExclusiveValue");
            }

            return GetFromCrypto(minValue, maxInclusiveValue);
        }

        private int GetFromCrypto(int minValue, int maxInclusiveValue)
        {
            var maxExclusiveValue = maxInclusiveValue + 1;

            var diff = (long) maxExclusiveValue - minValue;
            var upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);

            return (int) (minValue + (ui % diff));
        }

        private uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            var buffer = new byte[bytesNumber];
            rng.GetBytes(buffer);
            return buffer;
        }
    }
}