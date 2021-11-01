using System;
using System.Text.Json.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
    public class PageImage
    {
        [JsonPropertyName("source")]
        public Uri Source { get; set; }
        
        [JsonPropertyName("width")]
        public int Width { get; set; }
        
        [JsonPropertyName("height")]
        public int Height { get; set; }
        
    }
}