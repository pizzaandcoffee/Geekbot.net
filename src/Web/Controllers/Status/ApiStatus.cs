namespace Geekbot.Web.Controllers.Status;

public record ApiStatus
{
    public string GeekbotVersion { get; set; }

    public string ApiVersion { get; set; }

    public string Status { get; set; }
}