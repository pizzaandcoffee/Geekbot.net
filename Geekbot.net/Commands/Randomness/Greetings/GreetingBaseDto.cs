namespace Geekbot.net.Commands.Randomness.Greetings
{
    public class GreetingBaseDto
    {
        public string Language { get; set; }
        public string LanguageNative { get; set; }
        public string LanguageCode { get; set; }
        public string Script { get; set; }
        public GreetingDto Primary { get; set; }
    }
}