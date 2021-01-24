namespace Geekbot.Bot.Commands.Utils.Corona
{
    public record CoronaTotalDto
    {
        public string Country { get; set; }
        public decimal Cases { get; set; }
        public decimal Deaths { get; set; }
        public decimal Recovered { get; set; }
    }
}