using System.Globalization;
using System.Reflection;
using Geekbot.Core;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.Logger;
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

        private readonly IGeekbotLogger _logger;

        private readonly Dictionary<CommandType, Dictionary<string, Type>> _commands = new() {
            { CommandType.Message, new Dictionary<string, Type>() },
            { CommandType.User, new Dictionary<string, Type>() },
            { CommandType.ChatInput, new Dictionary<string, Type>() },
        };

        public Dictionary<string, Command> CommandsInfo { get; init; }

        public InteractionCommandManager(IServiceProvider provider, IGuildSettingsManager guildSettingsManager, IGeekbotLogger logger)
        {
            _provider = provider;
            _guildSettingsManager = guildSettingsManager;
            _logger = logger;

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

        private async Task HandleInteraction(Interaction interaction, InteractionBase command)
        {
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

            try
            {
                await HttpAbstractions.Patch(
                    new Uri($"https://discord.com/api/v8/webhooks/{interaction.ApplicationId}/{interaction.Token}/messages/@original"),
                    response.Data
                );
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Interaction, "Failed to send interaction response", e);
                throw;
            }
        }

        public InteractionResponse? RunCommand(Interaction interaction)
        {
            var type = _commands[interaction.Data.Type][interaction.Data.Name];
            var command = ActivatorUtilities.CreateInstance(_provider, type) as InteractionBase;

            if (command == null)
            {
                return null;
            }
            
            Task.Run(() => HandleInteraction(interaction, command).ConfigureAwait(false));

            return new InteractionResponse()
            {
                Type = InteractionResponseType.DeferredChannelMessageWithSource
            };
        }
    }
}