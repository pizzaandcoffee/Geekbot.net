using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class GuildSettingsModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        public bool Ping { get; set; }
        
        public bool Hui { get; set; }
        
        public long ModChannel { get; set; }
        
        public string WelcomeMessage { get; set; }
        
        public bool ShowDelete { get; set; }
        
        public bool ShowLeave { get; set; }
        
        public string WikiLang { get; set; }
        
        public string Language { get; set; }
    }
}