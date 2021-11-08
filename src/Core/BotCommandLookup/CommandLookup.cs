using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Discord.Commands;

namespace Geekbot.Core.BotCommandLookup;

public class CommandLookup
{
    private readonly Assembly _assembly;

    public CommandLookup(Assembly assembly)
    {
        _assembly = assembly;
    }
    
    public List<CommandInfo> GetCommands()
    {
        var commands = SearchCommands(_assembly);
        var result = new List<CommandInfo>();
        commands.ForEach(x => GetCommandDefinition(ref result, x));

        return result;
    }

    private List<TypeInfo> SearchCommands(Assembly assembly)
    {
        bool IsLoadableModule(TypeInfo info) => info.DeclaredMethods.Any(x => x.GetCustomAttribute<CommandAttribute>() != null || x.GetCustomAttribute<GroupAttribute>() != null);
        return assembly
            .DefinedTypes
            .Where(typeInfo => typeInfo.IsPublic || typeInfo.IsNestedPublic)
            .Where(IsLoadableModule)
            .ToList();
    }

    private void GetCommandDefinition(ref List<CommandInfo> commandInfos, TypeInfo commandType)
    {
        var methods = commandType
            .GetMethods()
            .Where(x => x.GetCustomAttribute<CommandAttribute>() != null)
            .ToList();

        var commandGroup = (commandType.GetCustomAttributes().FirstOrDefault(attr => attr is GroupAttribute) as GroupAttribute)?.Prefix;

        foreach (var command in methods)
        {
            var commandInfo = new CommandInfo()
            {
                Parameters = new Dictionary<string, ParameterInfo>(),
            };
            
            foreach (var attr in command.GetCustomAttributes())
            {
                
                switch (attr)
                {
                    case SummaryAttribute name:
                        commandInfo.Summary = name.Text;
                        break;
                    case CommandAttribute name:
                        commandInfo.Name = string.IsNullOrEmpty(commandGroup) ? name.Text : $"{commandGroup} {name.Text}";
                        break;
                    case AliasAttribute name:
                        commandInfo.Aliases = name.Aliases.ToList() ?? new List<string>();
                        break;
                }
            }

            foreach (var param in command.GetParameters())
            {
                var paramName = param.Name ?? string.Empty;
                var paramInfo = new ParameterInfo()
                {
                    Summary = param.GetCustomAttribute<SummaryAttribute>()?.Text ?? string.Empty,
                    Type = param.ParameterType.Name,
                    DefaultValue = param.DefaultValue?.ToString() ?? string.Empty
                };
                commandInfo.Parameters.Add(paramName, paramInfo);
            }
            
            if (!string.IsNullOrEmpty(commandInfo.Name))
            {
                commandInfos.Add(commandInfo);                    
            }
        }
    }
}