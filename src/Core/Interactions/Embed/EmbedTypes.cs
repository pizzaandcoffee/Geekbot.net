using System.Runtime.Serialization;

namespace Geekbot.Core.Interactions.Embed
{
    /// <summary>
    /// Embed types are "loosely defined" and, for the most part, are not used by our clients for rendering.
    /// Embed attributes power what is rendered.
    /// Embed types should be considered deprecated and might be removed in a future API version.
    /// </summary>
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object-embed-types" />
    public enum EmbedTypes
    {
        /// <summary>
        /// generic embed rendered from embed attributes
        /// </summary>
        [EnumMember(Value = "rich")]
        Rich,
        
        /// <summary>
        /// image embed
        /// </summary>
        [EnumMember(Value = "image")]
        Image,
        
        /// <summary>
        /// video embed
        /// </summary>
        [EnumMember(Value = "video")]
        Video,
        
        /// <summary>
        /// animated gif image embed rendered as a video embed
        /// </summary>
        [EnumMember(Value = "gifv")]
        Gifv,
        
        /// <summary>
        /// article embed
        /// </summary>
        [EnumMember(Value = "article")]
        Article,
        
        /// <summary>
        /// link embed
        /// </summary>
        [EnumMember(Value = "link")]
        Link,
    }
}