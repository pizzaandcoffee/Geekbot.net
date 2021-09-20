using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Geekbot.Core.Interactions.Resolved
{
    public record Message
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }
        
        [JsonPropertyName("guild_id")]
        public string GuildId { get; set; }
        
        [JsonPropertyName("author")]
        public User Author { get; set; }
        
        [JsonPropertyName("member")]
        public Member Member { get; set; }
        
        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
        
        [JsonPropertyName("edited_timestamp")]
        public DateTime? EditedTimestamp { get; set; }
        
        [JsonPropertyName("tts")]
        public bool Tts { get; set; }
        
        [JsonPropertyName("mention_everyone")]
        public bool MentionEveryone { get; set; }
        
        [JsonPropertyName("mentions")]
        public List<User> Mentions { get; set; }
        
        [JsonPropertyName("mention_roles")]
        public List<Role> MentionRoles { get; set; }
        
        [JsonPropertyName("mention_channels")]
        public List<Channel> MentionChannels { get; set; }
        
        [JsonPropertyName("attachments")]
        public List<Attachment> Attachments { get; set; }
        
        [JsonPropertyName("embeds")]
        public List<Embed.Embed> Embeds { get; set; }
        
        [JsonPropertyName("reactions")]
        public List<Reaction> Reactions { get; set; }
        
        // [JsonPropertyName("nonce")]
        // public string Nonce { get; set; }
        
        [JsonPropertyName("pinned")]
        public bool Pinned { get; set; }
        
        [JsonPropertyName("webhook_id")]
        public string WebhookId { get; set; }
        
        [JsonPropertyName("type")]
        public MessageType Type { get; set; }
        
        // [JsonPropertyName("activity")]
        // public string Activity { get; set; }
        
        // [JsonPropertyName("application")]
        // public string Application { get; set; }
        
        [JsonPropertyName("application_id")]
        public string ApplicationId { get; set; }
        
        // [JsonPropertyName("message_reference")]
        // public string MessageReference { get; set; }
        
        [JsonPropertyName("flags")]
        public int Flags { get; set; }
        
        [JsonPropertyName("referenced_message")]
        public Message ReferencedMessage { get; set; }
        
        [JsonPropertyName("interaction")]
        public MessageInteraction Interaction { get; set; }
        
        [JsonPropertyName("thread")]
        public Channel Thread { get; set; }
        
        // [JsonPropertyName("components")]
        // public string Components { get; set; }
        
        // [JsonPropertyName("sticker_items")]
        // public string StickerItems { get; set; }
        
        // [JsonPropertyName("stickers")]
        // public string Stickers { get; set; }
    }
}