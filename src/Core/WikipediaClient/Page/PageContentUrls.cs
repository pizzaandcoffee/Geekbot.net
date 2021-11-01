using System;
using System.Text.Json.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
    public class PageContentUrls
    {
        [JsonPropertyName("page")]
        public Uri Page { get; set; }
        
        [JsonPropertyName("revisions")]
        public Uri Revisions { get; set; }
        
        [JsonPropertyName("edit")]
        public Uri Edit { get; set; }
        
        [JsonPropertyName("talk")]
        public Uri Talk { get; set; }
    }
}