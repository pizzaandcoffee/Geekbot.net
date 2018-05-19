using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class PollModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public long GuildId { get; set; }
        
        [Required]
        public long ChannelId { get; set; }
        
        public string Question { get; set; }
        
        public long Creator { get; set; }
        
        public long MessageId { get; set; }

        public List<PollQuestionModel> Options { get; set; }

        public bool IsFinshed { get; set; }
    }
}