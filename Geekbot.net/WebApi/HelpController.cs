using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.net.WebApi
{
    [Route("v1/commands")]
    public class HelpController : Controller
    {
        [HttpGet()]
        public List<CommandDto> getHelp()
        {
            var commands = getCommands().Result;

            var commandList = new List<CommandDto>();
            foreach (var cmd in commands.Commands)
            {
                var cmdParamsObj = new List<CommandParamDto>();
                foreach (var cmdParam in cmd.Parameters)
                {
                    var singleParamObj = new CommandParamDto()
                    {
                        Summary = cmdParam.Summary,
                        Default = cmdParam?.DefaultValue?.ToString() ?? null,
                        Type = cmdParam?.Type?.ToString()
                    };
                    cmdParamsObj.Add(singleParamObj);
                }
                    
                var param = string.Join(", !", cmd.Aliases);
                var cmdObj = new CommandDto()
                {
                    Name = cmd.Name,
                    Summary = cmd.Summary,
                    Category = cmd.Remarks ?? CommandCategories.Uncategorized,
                    IsAdminCommand = (param.Contains("admin")),
                    Aliases = cmd.Aliases.ToArray(),
                    Params = cmdParamsObj
                };
                commandList.Add(cmdObj);
            }
            return commandList;
        }
        
        private async Task<CommandService> getCommands()
        {
            var commands = new CommandService();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
            return commands;
        }
    }

    public class CommandDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Summary { get; set; }
        public bool IsAdminCommand { get; set; }
        public Array Aliases { get; set; }
        public List<CommandParamDto> Params { get; set; } 
    }

    public class CommandParamDto
    {
        public string Summary { get; set; }
        public string Default { get; set; }
        public string Type { get; set; }
    }
}