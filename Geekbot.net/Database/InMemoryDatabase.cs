using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Geekbot.net.Database
{
    public class InMemoryDatabase : DatabaseContext
    {
        private readonly string _name;

        public InMemoryDatabase(string name)
        {
            _name = name;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole())
                .UseInMemoryDatabase(databaseName: _name);
    }
}