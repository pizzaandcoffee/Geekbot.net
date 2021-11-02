using System;
using System.Threading.Tasks;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;
using Sentry;

namespace Geekbot.Core.Interactions
{
    public abstract class InteractionBase : IInteractionBase
    {
        public virtual void BeforeExecute()
        {
            
        }

        public virtual void AfterExecute()
        {
            
        }
        
        public virtual void OnException(Exception exception)
        {
            if (!SentrySdk.IsEnabled) return;
            SentrySdk.CaptureException(exception);
        }

        public virtual InteractionResponse GetExceptionResponse()
        {
            return new InteractionResponse()
            {
                Type = InteractionResponseType.ChannelMessageWithSource,
                Data = new()
                {
                    Content = Localization.Internal.SomethingWentWrong
                }
            };
        }

        public abstract Command GetCommandInfo();
        public abstract Task<InteractionResponse> Exec(Interaction interaction);
        
        void IInteractionBase.BeforeExecute() => this.BeforeExecute();
        void IInteractionBase.AfterExecute() => this.AfterExecute();
        void IInteractionBase.OnException(Exception e) => this.OnException(e);
        InteractionResponse IInteractionBase.GetExceptionResponse() => this.GetExceptionResponse();
    }
}