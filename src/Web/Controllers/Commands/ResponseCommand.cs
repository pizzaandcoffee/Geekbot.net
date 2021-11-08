using System.Text.Json.Serialization;

namespace Geekbot.Web.Controllers.Commands;

public record ResponseCommand
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("summary")]
    public string Summary { get; set; }
    
    [JsonPropertyName("isAdminCommand")]
    public bool IsAdminCommand { get; set; }
    
    [JsonPropertyName("aliases")]
    public List<string> Aliases { get; set; }
    
    [JsonPropertyName("params")]
    public List<ResponseCommandParam> Params { get; set; }
}