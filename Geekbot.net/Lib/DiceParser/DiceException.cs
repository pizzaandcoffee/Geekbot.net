using System;

namespace Geekbot.net.Lib.DiceParser
{
    public class DiceException : Exception
    {
        public DiceException(string message) : base(message)
        {
        }
        
        public string DiceName { get; set; }
    }
}