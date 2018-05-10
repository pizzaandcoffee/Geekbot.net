using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
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
        
        public string[] UsedNames { get; set; }
    }
}