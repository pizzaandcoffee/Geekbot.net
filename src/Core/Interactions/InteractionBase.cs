using System.Threading.Tasks;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;

namespace Geekbot.Core.Interactions
{
    public abstract class InteractionBase : IInteractionBase
    {
        protected virtual void BeforeExecute()
        {
        }

        protected virtual void AfterExecute()
        {
            
        }
        
        protected virtual void OnException()
        {
            
        }

        public abstract Command GetCommandInfo();
        public abstract Task<InteractionResponse> Exec(InteractionData interaction);
        
        void IInteractionBase.BeforeExecute() => this.BeforeExecute();
        void IInteractionBase.AfterExecute() => this.AfterExecute();
        void IInteractionBase.OnException() => this.OnException();
    }
}