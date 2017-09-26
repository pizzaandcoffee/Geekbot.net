using System;
using System.IO;
using Serilog;

namespace Geekbot.net.Lib
{
    internal class FortunesProvider : IFortunesProvider
    {
        private readonly string[] fortuneArray;
        private readonly Random rnd;
        private readonly int totalFortunes;

        public FortunesProvider(Random rnd, ILogger logger)
        {
            var path = Path.GetFullPath("./Storage/fortunes");
            if (File.Exists(path))
            {
                var rawFortunes = File.ReadAllText(path);
                fortuneArray = rawFortunes.Split("%");
                totalFortunes = fortuneArray.Length;
                this.rnd = rnd;
                logger.Information($"-- Loaded {totalFortunes} Fortunes");
            }
            else
            {
                logger.Error("Fortunes File not found");
                logger.Error($"Path should be {path}");
            }
        }

        public string GetRandomFortune()
        {
            return fortuneArray[rnd.Next(0, totalFortunes)];
        }
    }

    public interface IFortunesProvider
    {
        string GetRandomFortune();
    }
}