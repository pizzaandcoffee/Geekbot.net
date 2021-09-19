namespace Geekbot.Core.Interactions.ApplicationCommand
{
    /// <see href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-types"/>
    public enum CommandType
    {
        /// <summary>
        /// Slash commands; a text-based command that shows up when a user types /
        /// </summary>
        ChatInput = 1,
        
        /// <summary>
        /// A UI-based command that shows up when you right click or tap on a user
        /// </summary>
        User = 2,
        
        /// <summary>
        /// A UI-based command that shows up when you right click or tap on a message
        /// </summary>
        Message = 3,
    }
}