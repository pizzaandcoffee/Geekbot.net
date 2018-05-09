using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Geekbot.net.Database
{
    public class QuoteModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int InternalId { get; set; }
        
        [Required]
        public ulong GuildId { get; set; }
        
        [Required]
        public ulong UserId { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }
        
        public string Quote { get; set; }
        
        public string Image { get; set; }
    }
}