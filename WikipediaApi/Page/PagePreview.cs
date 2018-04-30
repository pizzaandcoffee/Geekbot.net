using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WikipediaApi.Page
{
    public class PagePreview
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PageTypes Type { get; set; }
        
        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("displaytitle")]
        public string Displaytitle { get; set; }
        
        [JsonProperty("namespace")]
        public PageNamespace Namespace { get; set; }
        
        [JsonProperty("titles")]
        public PageTitles Titles { get; set; }
        
        [JsonProperty("pageid")]
        public ulong Pageid { get; set; }
        
        [JsonProperty("thumbnail")]
        public PageImage Thumbnail { get; set; }
        
        [JsonProperty("originalimage")]
        public PageImage Originalimage { get; set; }
        
        [JsonProperty("lang")]
        public string Lang { get; set; }
        
        [JsonProperty("dir")]
        public string Dir { get; set; }
        
        [JsonProperty("revision")]
        public ulong Revision { get; set; }
        
        [JsonProperty("tid")]
        public string Tid { get; set; }
        
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("coordinates")]
        public PageCoordinates Coordinates { get; set; }
        
        [JsonProperty("content_urls")]
        public PageContentUrlCollection ContentUrls { get; set; }
        
        [JsonProperty("api_urls")]
        public PageApiUrls ApiUrls { get; set; }
        
        [JsonProperty("extract")]
        public string Extract { get; set; }
        
        [JsonProperty("extract_html")]
        public string ExtractHtml { get; set; }
    }
}