using System;
using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Utils.Changelog
{
    public class CommitAuthorDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("date")]
        public DateTimeOffset Date { get; set; }
    }
}