using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.Embed
{
    /// <see href="https://discord.com/developers/docs/resources/channel#embed-object" />
    public class Embed
    {
        /// <summary>
        /// title of embed
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// type of embed (always "rich" for webhook embeds)
        /// </summary>
        [JsonPropertyName("type")]
        public EmbedTypes Type { get; set; } = EmbedTypes.Rich;
        
        /// <summary>
        /// description of embed
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// url of embed
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// timestamp of embed content
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }
        
        /// <summary>
        /// color code of the embed
        /// </summary>
        [JsonPropertyName("color")]
        public uint Color { get; set; }
        
        /// <summary>
        /// footer information
        /// </summary>
        [JsonPropertyName("footer")]
        public EmbedFooter Footer { get; set; }
        
        /// <summary>
        /// image information
        /// </summary>
        [JsonPropertyName("image")]
        public EmbedImage Image { get; set; }
        
        /// <summary>
        ///	thumbnail information
        /// </summary>
        [JsonPropertyName("thumbnail")]
        public EmbedThumbnail Thumbnail { get; set; }
        
        /// <summary>
        /// video information
        /// </summary>
        [JsonPropertyName("video")]
        public EmbedVideo Video { get; set; }
        
        /// <summary>
        /// provider information
        /// </summary>
        [JsonPropertyName("provider")]
        public EmbedProvider Provider { get; set; }
        
        /// <summary>
        /// author information
        /// </summary>
        [JsonPropertyName("author")]
        public EmbedAuthor Author { get; set; }

        /// <summary>
        /// fields information
        /// </summary>
        [JsonPropertyName("fields")]
        public List<EmbedField> Fields { get; set; } = new List<EmbedField>();

        public void AddField(string name, string value, bool inline = false)
        {
            Fields.Add(new EmbedField()
            {
                Name = name,
                Value = value,
                Inline = inline
            });
        }

        public void AddInlineField(string name, string value)
        {
            AddField(name, value, true);
        }

        public void SetColor(Color c)
        {
            Color = (uint)(c.R << 16 | c.G << 8 | c.B);
        }
    }
}