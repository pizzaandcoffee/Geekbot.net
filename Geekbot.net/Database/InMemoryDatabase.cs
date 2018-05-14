using Microsoft.EntityFrameworkCore;

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
            => optionsBuilder.UseInMemoryDatabase(_name);
    }
}