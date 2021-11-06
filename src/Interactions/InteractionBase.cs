using Geekbot.Interactions.ApplicationCommand;
using Geekbot.Interactions.Request;
using Geekbot.Interactions.Response;
using Sentry;
using Localization = Geekbot.Core.Localization;

namespace Geekbot.Interactions
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
            return SimpleResponse(Localization.Internal.SomethingWentWrong);
        }

        protected InteractionResponse SimpleResponse(string message) => new InteractionResponse()
            {
                Type = InteractionResponseType.ChannelMessageWithSource,
                Data = new()
                {
                    Content = message
                }
            };

        protected InteractionResponse SimpleResponse(Embed.Embed embed) => new InteractionResponse()
            {
                Type = InteractionResponseType.ChannelMessageWithSource,
                Data = new ()
                {
                    Content = string.Empty,
                    Embeds = new ()
                    {
                        embed
                    }
                }
            };

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