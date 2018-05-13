using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class RoleSelfServiceModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        public long RoleId { get; set; }
        
        public string WhiteListName { get; set; }
    }
}