using System;
using System.IO;

namespace Geekbot.net.Lib
{
    public class CheckEmImageProvider : ICheckEmImageProvider
    {
        private readonly string[] checkEmImageArray;
        private readonly Random rnd;
        private readonly int totalCheckEmImages;

        public CheckEmImageProvider(Random rnd)
        {
            var path = Path.GetFullPath("./Storage/checkEmPics");
            if (File.Exists(path))
            {
                var rawCheckEmPics = File.ReadAllText(path);
                checkEmImageArray = rawCheckEmPics.Split("\n");
                totalCheckEmImages = checkEmImageArray.Length;
                this.rnd = rnd;
                Console.WriteLine($"-- Loaded {totalCheckEmImages} CheckEm Images");
            }
            else
            {
                Console.WriteLine("checkEmPics File not found");
                Console.WriteLine($"Path should be {path}");
            }
        }

        public string GetRandomCheckEmPic()
        {
            return checkEmImageArray[rnd.Next(0, totalCheckEmImages)];
        }
    }

    public interface ICheckEmImageProvider
    {
        string GetRandomCheckEmPic();
    }
}