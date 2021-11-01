using System;
using System.Text.Json.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
    public class PageApiUrls
    {
        [JsonPropertyName("summary")]
        public Uri Summary { get; set; }
        
        [JsonPropertyName("metadata")]
        public Uri Metadata { get; set; }
        
        [JsonPropertyName("references")]
        public Uri References { get; set; }
        
        [JsonPropertyName("media")]
        public Uri Media { get; set; }
        
        [JsonPropertyName("edit_html")]
        public Uri EditHtml { get; set; }
        
        [JsonPropertyName("talk_page_html")]
        public Uri TalkPageHtml { get; set; }
    }
}