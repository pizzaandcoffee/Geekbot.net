using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Integrations.UbranDictionary
{
    internal class UrbanResponseDto
    {
        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }
        
        [JsonPropertyName("list")]
        public List<UrbanListItemDto> List { get; set; }
    }
}