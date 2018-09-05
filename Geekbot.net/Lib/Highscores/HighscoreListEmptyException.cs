using System;

namespace Geekbot.net.Lib.Highscores
{
    public class HighscoreListEmptyException : Exception
    {
        public HighscoreListEmptyException() {}

        public HighscoreListEmptyException(string message) : base(message) {}

        public HighscoreListEmptyException(string message, Exception inner) : base(message, inner) {}
    }
}