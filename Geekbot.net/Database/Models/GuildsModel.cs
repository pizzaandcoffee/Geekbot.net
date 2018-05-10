using System;
using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class GuildsModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public long Owner { get; set; }
        
        public string IconUrl { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; }
    }
}