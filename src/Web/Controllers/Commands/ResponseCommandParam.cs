using System.Text.Json.Serialization;

namespace Geekbot.Web.Controllers.Commands;

public record ResponseCommandParam
{
    [JsonPropertyName("summary")]
    public string Summary { get; set; }
    
    [JsonPropertyName("default")]
    public string Default { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
}