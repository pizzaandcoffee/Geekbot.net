using System.Collections.Generic;
using System.Threading.Tasks;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;

namespace Geekbot.Core.Interactions
{
    public interface IInteractionCommandManager
    {
        Dictionary<string, Command> CommandsInfo { get; init; }
        Task<InteractionResponse> RunCommand(Interaction interaction);
    }
}