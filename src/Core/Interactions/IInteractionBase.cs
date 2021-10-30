using System;
using Geekbot.Core.Interactions.Response;

namespace Geekbot.Core.Interactions
{
    public interface IInteractionBase
    {
        void BeforeExecute();
        void AfterExecute();
        void OnException(Exception e);
        InteractionResponse GetExceptionResponse();
    }
}