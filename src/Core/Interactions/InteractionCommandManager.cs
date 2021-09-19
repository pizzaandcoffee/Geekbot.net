using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Geekbot.Core.GlobalSettings;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;
using Geekbot.Core.Logger;

namespace Geekbot.Core.Interactions
{
    public class InteractionCommandManager : IInteractionCommandManager
    {
        private readonly Dictionary<string, Type> _commands = new();
        
        public Dictionary<string, Command> CommandsInfo { get; init; }

        public InteractionCommandManager()
        {
            var interactions = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(InteractionBase)))
                .ToList();

            CommandsInfo = new Dictionary<string, Command>();
            
            foreach (var interactionType in interactions)
            {
                var instance = (InteractionBase)Activator.CreateInstance(interactionType);
                var commandInfo = instance.GetCommandInfo();
                _commands.Add(commandInfo.Name, interactionType);
                CommandsInfo.Add(commandInfo.Name, commandInfo);
            }
        }

        public async Task<InteractionResponse> RunCommand(Interaction interaction)
        {
            var type = _commands[interaction.Data.Name];
            var command = (InteractionBase)Activator.CreateInstance(type);
            
            return await command.Exec(interaction.Data);
        }
    }
}