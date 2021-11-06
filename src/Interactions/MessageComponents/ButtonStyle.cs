namespace Geekbot.Interactions.MessageComponents
{
    /// <see href="https://discord.com/developers/docs/interactions/message-components#button-object-button-styles" />
    public enum ButtonStyle
    {
        /// <summary>
        /// blurple
        /// </summary>
        Primary = 1,
        
        /// <summary>
        /// grey
        /// </summary>
        Secondary = 2,
        
        /// <summary>
        /// green
        /// </summary>
        Success = 3,
        
        /// <summary>
        /// red
        /// </summary>
        Danger = 4,
        
        /// <summary>
        /// grey, navigates to a URL
        /// </summary>
        Link = 5
    }
}