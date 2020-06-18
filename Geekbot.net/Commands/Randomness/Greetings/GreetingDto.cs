namespace Geekbot.net.Commands.Randomness.Greetings
{
    public class GreetingDto
    {
        public string Text { get; set; } 
        public string Language { get; set; }
        public string Dialect { get; set; } = null;
        public string Romanization { get; set; } = null;
    }
}