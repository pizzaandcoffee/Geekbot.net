using System;

namespace Geekbot.Web.Commands;

public record RollTimeout
{
    public int LastGuess { get; set; }
    public DateTime GuessedOn { get; set; }
}