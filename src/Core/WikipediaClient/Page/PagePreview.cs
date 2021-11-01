using System;
using System.Text.Json.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
    public class PagePreview
    {
        [JsonPropertyName("type")]
        public PageTypes Type { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("displaytitle")]
        public string Displaytitle { get; set; }
        
        [JsonPropertyName("namespace")]
        public PageNamespace Namespace { get; set; }
        
        [JsonPropertyName("titles")]
        public PageTitles Titles { get; set; }
        
        [JsonPropertyName("pageid")]
        public ulong Pageid { get; set; }
        
        [JsonPropertyName("thumbnail")]
        public PageImage Thumbnail { get; set; }
        
        [JsonPropertyName("originalimage")]
        public PageImage Originalimage { get; set; }
        
        [JsonPropertyName("lang")]
        public string Lang { get; set; }
        
        [JsonPropertyName("dir")]
        public string Dir { get; set; }
        
        [JsonPropertyName("revision")]
        public string Revision { get; set; }
        
        [JsonPropertyName("tid")]
        public string Tid { get; set; }
        
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("coordinates")]
        public PageCoordinates Coordinates { get; set; }
        
        [JsonPropertyName("content_urls")]
        public PageContentUrlCollection ContentUrls { get; set; }
        
        [JsonPropertyName("api_urls")]
        public PageApiUrls ApiUrls { get; set; }
        
        [JsonPropertyName("extract")]
        public string Extract { get; set; }
        
        [JsonPropertyName("extract_html")]
        public string ExtractHtml { get; set; }
    }
}