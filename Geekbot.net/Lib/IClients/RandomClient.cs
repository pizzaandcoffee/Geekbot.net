using System;

namespace Geekbot.net.Lib.IClients
{

    public interface IRandomClient
    {
        Random Client { get; set; }
    }

    public sealed class RandomClient : IRandomClient
    {
        public RandomClient()
        {
            try
            {
                Client = new Random();
            }
            catch (Exception)
            {
                Console.WriteLine("Start Redis pls...");
                Environment.Exit(1);
            }
        }

        public Random Client { get; set; }
    }

}