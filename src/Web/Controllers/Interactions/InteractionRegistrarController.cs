using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Geekbot.Core;
using Geekbot.Core.GlobalSettings;
using Geekbot.Core.Interactions;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Logger;
using Geekbot.Web.Controllers.Interactions.Model;
using Microsoft.AspNetCore.Mvc;
using Sentry.Protocol;

namespace Geekbot.Web.Controllers.Interactions
{
    public class InteractionRegistrarController : Controller
    {
        private readonly IGeekbotLogger _logger;
        private readonly IInteractionCommandManager _interactionCommandManager;
        private readonly string _discordToken;
        // private readonly string _applicationId;
        private readonly Uri _guildCommandUri;

        public InteractionRegistrarController(IGlobalSettings globalSettings, IGeekbotLogger logger, IInteractionCommandManager interactionCommandManager)
        {
            _logger = logger;
            _interactionCommandManager = interactionCommandManager;
            _discordToken = globalSettings.GetKey("DiscordToken");
            var applicationId = globalSettings.GetKey("DiscordApplicationId");
            var runesPlayground = "131827972083548160";
            _guildCommandUri = new Uri($"https://discord.com/api/v8/applications/{applicationId}/guilds/{runesPlayground}/commands");
        }
        
        [HttpPost]
        [Route("/interactions/register")]
        public async Task<IActionResult> RegisterInteractions()
        {
            var registeredInteractions = await GetRegisteredInteractions();
            var operations = new InteractionRegistrationOperations();

            foreach (var (_, command) in _interactionCommandManager.CommandsInfo)
            {
                var existing = registeredInteractions.FirstOrDefault(i => i.Name == command.Name);
                if (existing == null)
                {
                    operations.Create.Add(command);
                }
                else
                {
                    operations.Update.Add(existing.Id, command);
                }
            }

            foreach (var registeredInteraction in registeredInteractions.Where(registeredInteraction => !_interactionCommandManager.CommandsInfo.Values.Any(c => c.Name == registeredInteraction.Name)))
            {
                operations.Remove.Add(registeredInteraction.Id);
            }

            // foreach (var (_, command) in _interactionCommandManager.CommandsInfo)
            // {
            //     try
            //     {
            //         var httpClient = HttpAbstractions.CreateDefaultClient();
            //         httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _discordToken);
            //         
            //         await HttpAbstractions.Post(_guildCommandUri, command, httpClient);
            //         
            //         _logger.Information(LogSource.Interaction, $"Registered Interaction: {command.Name}");
            //     }
            //     catch (Exception e)
            //     {
            //         _logger.Error(LogSource.Interaction, $"Failed to register Interaction: {command.Name}", e);
            //     }
            // }

            await Task.WhenAll(new[]
            {
                Create(operations.Create),
                Update(operations.Update),
                Remove(operations.Remove)
            });
            return Ok(operations);
        }

        private HttpClient CreateClientWithToken()
        {
            var httpClient = HttpAbstractions.CreateDefaultClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _discordToken);

            return httpClient;
        }
        
        private async Task Create(List<Command> commands)
        {
            foreach (var command in commands)
            {
                try
                {
                    await HttpAbstractions.Post(_guildCommandUri, command, CreateClientWithToken());
                    _logger.Information(LogSource.Interaction, $"Registered interaction: {command.Name}");
                }
                catch (Exception e)
                {
                    _logger.Error(LogSource.Interaction, $"Failed to register interaction: {command.Name}", e);
                }
            }
        }

        private async Task Update(Dictionary<string, Command> commands)
        {
            foreach (var (id, command) in commands)
            {
                try
                {
                    var updateUri = new Uri(_guildCommandUri.AbsoluteUri + $"/{id}");
                    await HttpAbstractions.Patch(updateUri, command, CreateClientWithToken());
                    _logger.Information(LogSource.Interaction, $"Updated Interaction: {command.Name}");
                }
                catch (Exception e)
                {
                    _logger.Error(LogSource.Interaction, $"Failed to update Interaction: {command.Name}", e);
                }
            }
        }

        private async Task Remove(List<string> commands)
        {
            foreach (var id in commands)
            {
                try
                {
                    var updateUri = new Uri(_guildCommandUri.AbsoluteUri + $"/{id}");
                    await HttpAbstractions.Delete(updateUri, CreateClientWithToken());
                    _logger.Information(LogSource.Interaction, $"Deleted interaction with ID: {id}");
                }
                catch (Exception e)
                {
                    _logger.Error(LogSource.Interaction, $"Failed to delete interaction with id: {id}", e);
                }
            }
        }

        private async Task<List<RegisteredInteraction>> GetRegisteredInteractions()
        {
            var httpClient = HttpAbstractions.CreateDefaultClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _discordToken);

            return await HttpAbstractions.Get<List<RegisteredInteraction>>(_guildCommandUri, httpClient);
        }
    }
}