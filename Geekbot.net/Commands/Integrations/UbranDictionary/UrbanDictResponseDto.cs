using System.Collections.Generic;

namespace Geekbot.net.Commands.Integrations.UbranDictionary
{
    internal class UrbanResponseDto
    {
        public string[] Tags { get; set; }
        public List<UrbanListItemDto> List { get; set; }
    }
}