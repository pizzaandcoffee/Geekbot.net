using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Utils.Changelog
{
    public class CommitInfoDto
    {
        [JsonPropertyName("author")]
        public CommitAuthorDto Author { get; set; }
        
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}