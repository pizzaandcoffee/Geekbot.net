using System;
using System.IO;
using Serilog;

namespace Geekbot.net.Lib
{
    public class PandaProvider : IPandaProvider
    {
        private readonly string[] PandaArray;
        private readonly Random rnd;
        private readonly int totalPandas;

        public PandaProvider(Random rnd, ILogger logger)
        {
            var path = Path.GetFullPath("./Storage/pandas");
            if (File.Exists(path))
            {
                var rawFortunes = File.ReadAllText(path);
                PandaArray = rawFortunes.Split("\n");
                totalPandas = PandaArray.Length;
                this.rnd = rnd;
                logger.Information($"[Geekbot] [Pandas] Loaded {totalPandas} Panda Images");
            }
            else
            {
                logger.Error("Pandas File not found");
                logger.Error($"Path should be {path}");
            }
        }

        public string GetRandomPanda()
        {
            return PandaArray[rnd.Next(0, totalPandas)];
        }
    }

    public interface IPandaProvider
    {
        string GetRandomPanda();
    }
}