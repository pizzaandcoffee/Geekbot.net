using System;
using System.Security.Cryptography;
using Anemonis.RandomOrg;
using Geekbot.Core.GlobalSettings;

namespace Geekbot.Core.RandomNumberGenerator
{
    public class RandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly RNGCryptoServiceProvider csp;
        private readonly bool _canUseRandomOrg;
        private readonly RandomOrgClient _randomOrgClient;

        public RandomNumberGenerator(IGlobalSettings globalSettings)
        {
            csp = new RNGCryptoServiceProvider();

            var randomOrgApiKey = globalSettings.GetKey("RandomOrgApiKey");
            if (!string.IsNullOrEmpty(randomOrgApiKey))
            {
                _canUseRandomOrg = true;
                _randomOrgClient = new RandomOrgClient(randomOrgApiKey);
            }
        }

        public int Next(int minValue, int maxInclusiveValue)
        {
            if (minValue == maxInclusiveValue)
            {
                return maxInclusiveValue;
            }
            
            if (minValue >= maxInclusiveValue)
            {
                throw new ArgumentOutOfRangeException("minValue must be lower than maxExclusiveValue");
            }
            
            if (_canUseRandomOrg)
            {
                try
                {
                    return GetFromRandomOrg(minValue, maxInclusiveValue);
                }
                catch
                {
                    // ignore
                }
            }

            return GetFromCrypto(minValue, maxInclusiveValue);
        }

        private int GetFromRandomOrg(int minValue, int maxInclusiveValue)
        {
            return _randomOrgClient
                .GenerateIntegersAsync(1, minValue, maxInclusiveValue, false)
                .Result
                .Random
                .Data[0];
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
            csp.GetBytes(buffer);
            return buffer;
        }
    }
}