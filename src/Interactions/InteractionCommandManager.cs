using System.Globalization;
using System.Reflection;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Interactions.ApplicationCommand;
using Geekbot.Interactions.Request;
using Geekbot.Interactions.Response;
using Microsoft.Extensions.DependencyInjection;

namespace Geekbot.Interactions
{
    public class InteractionCommandManager : IInteractionCommandManager
    {
        private readonly IServiceProvider _provider;
        
        private readonly IGuildSettingsManager _guildSettingsManager;

        private readonly Dictionary<CommandType, Dictionary<string, Type>> _commands = new() {
            { CommandType.Message, new Dictionary<string, Type>() },
            { CommandType.User, new Dictionary<string, Type>() },
            { CommandType.ChatInput, new Dictionary<string, Type>() },
        };

        public Dictionary<string, Command> CommandsInfo { get; init; }

        public InteractionCommandManager(IServiceProvider provider, IGuildSettingsManager guildSettingsManager)
        {
            _provider = provider;
            _guildSettingsManager = guildSettingsManager;
            var interactions = Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(InteractionBase)))
                .ToList();

            CommandsInfo = new Dictionary<string, Command>();
            
            foreach (var interactionType in interactions)
            {
                var instance = (InteractionBase)ActivatorUtilities.CreateInstance(provider, interactionType);
                var commandInfo = instance.GetCommandInfo();
                _commands[commandInfo.Type].Add(commandInfo.Name, interactionType);
                CommandsInfo.Add($"{commandInfo.Type}-{commandInfo.Name}", commandInfo);
            }
        }

        public async Task<InteractionResponse> RunCommand(Interaction interaction)
        {
            var type = _commands[interaction.Data.Type][interaction.Data.Name];
            var command = ActivatorUtilities.CreateInstance(_provider, type) as InteractionBase;

            if (command == null)
            {
                return null;
            }

            var guildSettings = _guildSettingsManager.GetSettings(ulong.Parse(interaction.GuildId));
            var language = guildSettings.Language;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(language);

            InteractionResponse response;
            try
            {
                command.BeforeExecute(interaction);
                response = await command.Exec(interaction);
            }
            catch (Exception e)
            {
                command.OnException(interaction, e);
                response = command.GetExceptionResponse(interaction);
            }
            finally
            {
                command.AfterExecute(interaction);
            }

            return response;
        }
    }
}