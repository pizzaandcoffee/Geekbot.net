using System;
using System.ComponentModel.DataAnnotations;

namespace Geekbot.Core.Database.Models
{
    public class QuoteModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int InternalId { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        [Required]
        public long UserId { get; set; }
        
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }
        
        public string Quote { get; set; }
        
        public string Image { get; set; }
    }
}