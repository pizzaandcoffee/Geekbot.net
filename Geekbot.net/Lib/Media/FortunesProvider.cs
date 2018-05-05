using System;
using System.IO;
using Geekbot.net.Lib.Logger;

namespace Geekbot.net.Lib.Media
{
    internal class FortunesProvider : IFortunesProvider
    {
        private readonly string[] _fortuneArray;
        private readonly int _totalFortunes;

        public FortunesProvider(IGeekbotLogger logger)
        {
            var path = Path.GetFullPath("./Storage/fortunes");
            if (File.Exists(path))
            {
                var rawFortunes = File.ReadAllText(path);
                _fortuneArray = rawFortunes.Split("%");
                _totalFortunes = _fortuneArray.Length;
                logger.Trace(LogSource.Geekbot, $"Loaded {_totalFortunes} Fortunes");
            }
            else
            {
                logger.Information(LogSource.Geekbot, $"Fortunes File not found at {path}");
            }
        }

        public string GetRandomFortune()
        {
            return _fortuneArray[new Random().Next(0, _totalFortunes)];
        }
    }
}