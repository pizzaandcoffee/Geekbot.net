using System.Collections.Generic;
using Discord;
using Geekbot.Web.Controllers.Interactions.Model.MessageComponents;

namespace Geekbot.Web.Controllers.Interactions.Model
{
    public record InteractionResponseData
    {
        public bool Tts { get; set; } = false;
        public string Content { get; set; }
        public List<string> Embeds { get; set; }
        public AllowedMentions AllowedMentions { get; set; }
        public int Flags { get; set; }
        public List<Component> Components { get; set; }
    }
}