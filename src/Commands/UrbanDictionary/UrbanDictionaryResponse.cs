using System.Text.Json.Serialization;

namespace Geekbot.Commands.UrbanDictionary;

public struct UrbanDictionaryResponse
{
    [JsonPropertyName("tags")]
    public string[] Tags { get; set; }
        
    [JsonPropertyName("list")]
    public List<UrbanDictionaryListItem?> List { get; set; }
}