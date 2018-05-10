using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class UserSettingsModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long UserId { get; set; }
        
        // stuff to be added in the future
    }
}