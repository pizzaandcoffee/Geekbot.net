using System;

namespace Geekbot.Bot.Commands.Games.Roll
{
    public class RollTimeout
    {
        public int LastGuess { get; set; }
        public DateTime GuessedOn { get; set; }
    }
}