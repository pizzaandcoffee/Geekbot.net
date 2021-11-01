using System.Text.Json.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
    public class PageCoordinates
    {
        [JsonPropertyName("lat")]
        public float Lat { get; set; }
        
        [JsonPropertyName("lon")]
        public float Lon { get; set; }
    }
}