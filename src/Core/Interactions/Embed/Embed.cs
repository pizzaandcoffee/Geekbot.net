using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Discord;
using Color = System.Drawing.Color;

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

        [JsonIgnore]
        private Color _color { get; set; } = System.Drawing.Color.Black;
        
        /// <summary>
        /// color code of the embed
        /// </summary>
        [JsonPropertyName("color")]
        public uint Color => (uint)(_color.R << 16 | _color.G << 8 | _color.B);

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
        public List<EmbedField> Fields { get; init; } = new List<EmbedField>();

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
            _color = c;
        }

        public EmbedBuilder ToDiscordNetEmbed()
        {
            var eb = new EmbedBuilder();
            if (!string.IsNullOrEmpty(Title)) eb.Title = Title;
            if (!string.IsNullOrEmpty(Description)) eb.Description = Description;
            if (!string.IsNullOrEmpty(Url)) eb.Url = Url;
            if (Timestamp != null) eb.Timestamp = Timestamp;
            if (Color != 0) eb.WithColor(new Discord.Color(_color.R, _color.G, _color.B));
            if (Footer != null) eb.WithFooter(Footer.Text, Footer.IconUrl);
            if (Image != null) eb.WithImageUrl(Image.Url);
            if (Thumbnail != null) eb.WithThumbnailUrl(Thumbnail.Url);
            if (Author != null) eb.WithAuthor(Author.Name, Author.IconUrl, Author.Url);
            Fields.ForEach(field => eb.AddField(field.Name, field.Value, field.Inline));

            return eb;
        }
    }
}