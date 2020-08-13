using System;
using System.ComponentModel.DataAnnotations;

namespace Geekbot.Core.Database.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long UserId { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Discriminator { get; set; }
        
        public string AvatarUrl { get; set; }
        
        [Required]
        public bool IsBot { get; set; }
        
        public DateTimeOffset Joined { get; set; }
    }
}