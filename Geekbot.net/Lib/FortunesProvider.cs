using System;
using System.IO;

namespace Geekbot.net.Lib
{
    internal class FortunesProvider : IFortunesProvider
    {
        private readonly string[] fortuneArray;
        private readonly Random rnd;
        private readonly int totalFortunes;

        public FortunesProvider()
        {
            var path = Path.GetFullPath("./Storage/fortunes");
            if (File.Exists(path))
            {
                var rawFortunes = File.ReadAllText(path);
                fortuneArray = rawFortunes.Split("%");
                totalFortunes = fortuneArray.Length;
                rnd = new Random();
                Console.WriteLine($"- Loaded {totalFortunes} Fortunes");
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

        public string GetFortune(int id)
        {
            return fortuneArray[id];
        }
    }

    public interface IFortunesProvider
    {
        string GetRandomFortune();
        string GetFortune(int id);
    }
}