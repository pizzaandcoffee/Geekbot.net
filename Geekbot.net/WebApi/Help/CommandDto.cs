using System;
using System.Collections.Generic;

namespace Geekbot.net.WebApi.Help
{
    public class CommandDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }
        public bool IsAdminCommand { get; set; }
        public Array Aliases { get; set; }
        public List<CommandParamDto> Params { get; set; } 
    }
}