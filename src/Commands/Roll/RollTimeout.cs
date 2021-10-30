using System;

namespace Geekbot.Commands.Roll
{
    public record RollTimeout
    {
        public int LastGuess { get; set; }
        public DateTime GuessedOn { get; set; }
    }   
}