using System;
using System.Threading.Tasks;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;

namespace Geekbot.Core.Interactions
{
    public abstract class InteractionBase : IInteractionBase
    {
        public InteractionBase() {}
        
        public virtual void BeforeExecute()
        {
            
        }

        public virtual void AfterExecute()
        {
            
        }
        
        public virtual void OnException(Exception exception)
        {
            
        }

        public virtual InteractionResponse GetExceptionResponse()
        {
            return new InteractionResponse()
            {
                Type = InteractionResponseType.ChannelMessageWithSource,
                Data = new()
                {
                    Content = "Something went wrong :confused:"
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