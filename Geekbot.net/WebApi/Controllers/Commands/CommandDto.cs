using System.Collections.Generic;

namespace Geekbot.net.WebApi.Controllers.Commands
{
    public class CommandDto
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public bool IsAdminCommand { get; set; }
        public List<string> Aliases { get; set; }
        public List<CommandParamDto> Params { get; set; } 
    }
}