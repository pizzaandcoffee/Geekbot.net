using System;
using System.IO;

namespace Geekbot.net.Lib
{
    public class CheckEmImageProvider : ICheckEmImageProvider
    {
        private string[] checkEmImageArray;
        private int totalCheckEmImages;
        private Random rnd;

        public CheckEmImageProvider()
        {
            var path = Path.GetFullPath("./Storage/checkEmPics");
            if (File.Exists(path))
            {
                var rawCheckEmPics = File.ReadAllText(path);
                checkEmImageArray = rawCheckEmPics.Split("\n");
                totalCheckEmImages = checkEmImageArray.Length;
                rnd = new Random();
                Console.WriteLine($"- Loaded {totalCheckEmImages} CheckEm Images");
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