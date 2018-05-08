using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;

namespace Geekbot.net.Database
{
    public class QuoteModel
    {
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public string Quote { get; set; }
        public DateTime Time { get; set; }
        public string Image { get; set; }
    }
}