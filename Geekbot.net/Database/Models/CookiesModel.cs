using System;
using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class CookiesModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        [Required]
        public long UserId { get; set; }

        public int Cookies { get; set; } = 0;
        
        public DateTimeOffset? LastPayout { get; set; }
    }
}