using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Geekbot.net.Lib
{
    class FortunesProvider : IFortunesProvider
    {
        private string[] fortuneArray;
        private int totalFortunes;
        private Random rnd;

        public FortunesProvider()
        {
            var path = Path.GetFullPath("./Storage/fortunes");
            if (File.Exists(path))
            {
                var rawFortunes= File.ReadAllText(path);
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
