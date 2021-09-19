using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Geekbot.Web.Controllers.Interactions.Model
{
    public record Interaction
    {
        public string Id { get; set; }
        public string ApplicationId { get; init; }
        [Required]
        public InteractionType Type { get; set; }
        public InteractionData Data { get; init; }
        public string GuildId { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public List<InteractionOption> Options { get; init; }
        public bool DefaultPermission { get; init; }
        [Required]
        public int Version { get; set; }
    }
}