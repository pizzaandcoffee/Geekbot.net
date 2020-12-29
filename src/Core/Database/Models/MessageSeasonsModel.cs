using System.ComponentModel.DataAnnotations;

namespace Geekbot.Core.Database.Models
{
    public class MessageSeasonsModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        [Required]
        public long UserId { get; set; }
        
        [Required]
        public string Season { get; set; }
        
        public int MessageCount { get; set; }
    }
}