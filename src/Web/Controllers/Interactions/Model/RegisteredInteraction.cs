using System.Text.Json.Serialization;
using Geekbot.Interactions.Request;

namespace Geekbot.Web.Controllers.Interactions.Model;

public record RegisteredInteraction
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("application_id")]
    public string ApplicationId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("default_permission")]
    public bool DefaultPermission { get; set; }

    [JsonPropertyName("type")]
    public InteractionType Type { get; set; }

    [JsonPropertyName("guild_id")]
    public string GuildId { get; set; }
}