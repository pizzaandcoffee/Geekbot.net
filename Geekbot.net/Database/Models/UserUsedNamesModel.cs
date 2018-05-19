using System;
using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class UserUsedNamesModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        
        public DateTimeOffset FirstSeen { get; set; }
    }
}