using Geekbot.Interactions.Request;
using Geekbot.Interactions.Response;

namespace Geekbot.Interactions
{
    public interface IInteractionBase
    {
        void BeforeExecute(Interaction interaction);
        void AfterExecute(Interaction interaction);
        void OnException(Interaction interaction, Exception e);
        InteractionResponse GetExceptionResponse(Interaction interaction);
    }
}