using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class ShipsModel
    {
        [Key]
        public int Id { get; set; }
        
        public long FirstUserId { get; set; }
        
        public long SecondUserId { get; set; }
        
        public int Strength { get; set; }
    }
}