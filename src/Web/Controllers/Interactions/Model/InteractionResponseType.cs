namespace Geekbot.Web.Controllers.Interactions.Model
{
    public enum InteractionResponseType
    {
        Pong = 1,
        ChannelMessageWithSource = 4,
        DeferredChannelMessageWithSource = 5,
        DeferredUpdateMessage = 6,
        UpdateMessage = 7,
    }
}