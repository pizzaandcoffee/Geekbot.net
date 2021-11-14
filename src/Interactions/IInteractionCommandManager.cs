using Geekbot.Interactions.ApplicationCommand;
using Geekbot.Interactions.Request;
using Geekbot.Interactions.Response;

namespace Geekbot.Interactions
{
    public interface IInteractionCommandManager
    {
        Dictionary<string, Command> CommandsInfo { get; init; }
        InteractionResponse? RunCommand(Interaction interaction);
    }
}