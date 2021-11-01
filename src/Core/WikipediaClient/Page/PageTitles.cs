using System.Text.Json.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
    public class PageTitles
    {
        [JsonPropertyName("Canonical")]
        public string Canonical { get; set; }
        
        [JsonPropertyName("Normalized")]
        public string Normalized { get; set; }
        
        [JsonPropertyName("Display")]
        public string Display { get; set; }
    }
}