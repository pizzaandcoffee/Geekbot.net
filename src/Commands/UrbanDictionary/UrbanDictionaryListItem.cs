using System.Text.Json.Serialization;

namespace Geekbot.Commands.UrbanDictionary;

public record UrbanDictionaryListItem
{
    [JsonPropertyName("definition")]
    public string Definition { get; set; }
    
    [JsonPropertyName("permalink")]
    public string Permalink { get; set; }
    
    [JsonPropertyName("thumbs_up")]
    public int ThumbsUp { get; set; }
    
    [JsonPropertyName("word")]
    public string Word { get; set; }
    
    [JsonPropertyName("example")]
    public string Example { get; set; }
    
    [JsonPropertyName("thumbs_down")]
    public int ThumbsDown { get; set; }
}
