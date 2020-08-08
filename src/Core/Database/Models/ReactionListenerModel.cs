using System.ComponentModel.DataAnnotations;

namespace Geekbot.Core.Database.Models
{
    public class ReactionListenerModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        [Required]
        public long MessageId { get; set; }
        
        [Required]
        public long RoleId { get; set; }

        [Required]
        public string Reaction { get; set; }
    }
}