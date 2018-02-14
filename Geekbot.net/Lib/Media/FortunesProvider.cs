using System;
using System.IO;
using Serilog;

namespace Geekbot.net.Lib.Media
{
    internal class FortunesProvider : IFortunesProvider
    {
        private readonly string[] fortuneArray;
        private readonly int totalFortunes;

        public FortunesProvider(IGeekbotLogger logger)
        {
            var path = Path.GetFullPath("./Storage/fortunes");
            if (File.Exists(path))
            {
                var rawFortunes = File.ReadAllText(path);
                fortuneArray = rawFortunes.Split("%");
                totalFortunes = fortuneArray.Length;
                logger.Debug("Geekbot", "Loaded {totalFortunes} Fortunes");
            }
            else
            {
                logger.Information("Geekbot", $"Fortunes File not found at {path}");
            }
        }

        public string GetRandomFortune()
        {
            return fortuneArray[new Random().Next(0, totalFortunes)];
        }
    }

    public interface IFortunesProvider
    {
        string GetRandomFortune();
    }
}