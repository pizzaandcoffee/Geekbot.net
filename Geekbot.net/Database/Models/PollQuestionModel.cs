using System.ComponentModel.DataAnnotations;

namespace Geekbot.net.Database.Models
{
    public class PollQuestionModel
    {
        [Key]
        public int Id { get; set; }
        
        public int OptionId { get; set; }
        
        public string OptionText { get; set; }
        
        public int Votes { get; set; }
    }
}