using System;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;

namespace Geekbot.Core.Interactions
{
    public interface IInteractionBase
    {
        void BeforeExecute(Interaction interaction);
        void AfterExecute(Interaction interaction);
        void OnException(Interaction interaction, Exception e);
        InteractionResponse GetExceptionResponse(Interaction interaction);
    }
}