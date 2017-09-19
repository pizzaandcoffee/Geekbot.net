using System;
using System.IO;

namespace Geekbot.net.Lib
{
    internal class FortunesProvider : IFortunesProvider
    {
        private readonly string[] fortuneArray;
        private readonly Random rnd;
        private readonly int totalFortunes;

        public FortunesProvider(Random rnd)
        {
            var path = Path.GetFullPath("./Storage/fortunes");
            if (File.Exists(path))
            {
                var rawFortunes = File.ReadAllText(path);
                fortuneArray = rawFortunes.Split("%");
                totalFortunes = fortuneArray.Length;
                this.rnd = rnd;
                Console.WriteLine($"-- Loaded {totalFortunes} Fortunes");
            }
            else
            {
                Console.WriteLine("Fortunes File not found");
                Console.WriteLine($"Path should be {path}");
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