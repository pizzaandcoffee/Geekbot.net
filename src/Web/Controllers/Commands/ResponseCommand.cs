namespace Geekbot.Web.Controllers.Commands;

public record ResponseCommand
{
    public string Name { get; set; }
    public string Summary { get; set; }
    public bool IsAdminCommand { get; set; }
    public List<string> Aliases { get; set; }
    public List<ResponseCommandParam> Params { get; set; }
}