using System;
using System.ComponentModel.DataAnnotations;

namespace Geekbot.Core.Database.Models
{
    public class KarmaModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        [Required]
        public long UserId { get; set; }
        
        public int Karma { get; set; }
        
        public DateTimeOffset TimeOut { get; set; }
    }
}