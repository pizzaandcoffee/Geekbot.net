using System.ComponentModel.DataAnnotations;

namespace Geekbot.Core.Database.Models
{
    public class MessagesModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        [Required]
        public long UserId { get; set; }
        
        public int MessageCount { get; set; }
    }
}