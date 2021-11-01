using System.Text.Json.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
    public class PageNamespace
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}