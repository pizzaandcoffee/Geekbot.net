using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Geekbot.net.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<QuoteModel> Quotes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole())
//                .UseInMemoryDatabase(databaseName: "Geekbot");
                .UseMySql(@"Server=localhost;database=geekbot;uid=geekbot;");
    }
}