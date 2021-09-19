namespace Geekbot.Core.Interactions.ApplicationCommand
{
    /// <see href="https://discord.com/developers/docs/interactions/application-commands#application-command-object-application-command-option-type"/>
    public enum OptionType
    {
        SubCommand = 1,
        
        SubCommandGroup = 2,
        
        String = 3,
        
        /// <summary>
        /// Any integer between -2^53 and 2^53
        /// </summary>
        Integer = 4,
        
        Boolean = 5,
        
        User = 6,
        
        /// <summary>
        /// Includes all channel types + categories
        /// </summary>
        Channel = 7,
        
        Role = 8,
        
        /// <summary>
        /// Includes users and roles
        /// </summary>
        Mentionable = 9,
        
        /// <summary>
        /// Any double between -2^53 and 2^53
        /// </summary>
        Number = 10,
    }
}