using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class GlobalsModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Value { get; set; }
        
        public string Meta { get; set; }
    }
}