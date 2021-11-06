namespace Geekbot.Web.Controllers.Commands;

public record ResponseCommandParam
{
    public string Summary { get; set; }
    public string Default { get; set; }
    public string Type { get; set; }
}