using System.Collections.Generic;

namespace Geekbot.Core.BotCommandLookup;

public struct CommandInfo
{
    public string Name { get; set; }
    public Dictionary<string, ParameterInfo> Parameters { get; set; }
    public List<string> Aliases { get; set; }
    public string Summary { get; set; }
}