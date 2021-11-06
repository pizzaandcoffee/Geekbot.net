namespace Geekbot.Interactions.MessageComponents
{
    /// <see href="https://discord.com/developers/docs/interactions/message-components#component-object-component-types" />
    public enum ComponentType
    {
        /// <summary>
        /// A container for other components
        /// </summary>
        ActionRow = 1,
        
        /// <summary>
        /// A button object
        /// </summary>
        Button = 2,
        
        /// <summary>
        /// A select menu for picking from choices
        /// </summary>
        SelectMenu = 3
    }
}