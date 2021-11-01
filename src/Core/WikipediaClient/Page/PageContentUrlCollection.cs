using System.Text.Json.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
    public class PageContentUrlCollection
    {
        [JsonPropertyName("desktop")]
        public PageContentUrls Desktop { get; set; }
        
        [JsonPropertyName("mobile")]
        public PageContentUrls Mobile { get; set; }
    }
}