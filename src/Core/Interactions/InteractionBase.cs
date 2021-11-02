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
        public virtual void BeforeExecute(Interaction interaction)
        {
            
        }

        public virtual void AfterExecute(Interaction interaction)
        {
            
        }
        
        public virtual void OnException(Interaction interaction, Exception exception)
        {
            if (!SentrySdk.IsEnabled) return;
            SentrySdk.CaptureException(exception);
        }

        public virtual InteractionResponse GetExceptionResponse(Interaction interaction)
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
        
        void IInteractionBase.BeforeExecute(Interaction interaction)
            => this.BeforeExecute(interaction);
        void IInteractionBase.AfterExecute(Interaction interaction)
            => this.AfterExecute(interaction);
        void IInteractionBase.OnException(Interaction interaction, Exception e)
            => this.OnException(interaction, e);
        InteractionResponse IInteractionBase.GetExceptionResponse(Interaction interaction)
            => this.GetExceptionResponse(interaction);
    }
}