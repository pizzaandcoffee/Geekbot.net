using System.Text.Json.Serialization;

namespace Geekbot.Bot.Commands.Utils.Changelog
{
    public class CommitDto
    {
        [JsonPropertyName("commit")]
        public CommitInfoDto Commit { get; set; }
    }
}