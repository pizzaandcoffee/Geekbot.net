namespace Geekbot.Interactions.Request
{
    /// <see href="https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-object-interaction-type" />
    public enum InteractionType
    {
        Ping = 1,
        ApplicationCommand = 2,
        MessageComponent = 3,
    }
}