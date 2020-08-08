using System.ComponentModel.DataAnnotations;

namespace Geekbot.Core.Database.Models
{
    public class GuildSettingsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public long GuildId { get; set; }

        public bool Ping { get; set; } = false;

        public bool Hui { get; set; } = false;

        public long ModChannel { get; set; } = 0;

        public string WelcomeMessage { get; set; }

        public long WelcomeChannel { get; set; }

        public bool ShowDelete { get; set; } = false;

        public bool ShowLeave { get; set; } = false;

        public string WikiLang { get; set; } = "en";

        public string Language { get; set; } = "EN";
    }
}