using System;

namespace Geekbot.net.Commands.Games.Roll
{
    public class RollTimeout
    {
        public int LastGuess { get; set; }
        public DateTime GuessedOn { get; set; }
    }
}