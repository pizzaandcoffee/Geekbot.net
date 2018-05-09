using Microsoft.EntityFrameworkCore;

namespace Geekbot.net.Database
{
    public class SqlDatabase : DatabaseContext
    {
        private readonly SqlConnectionString _connectionString;

        public SqlDatabase(SqlConnectionString connectionString)
        {
            _connectionString = connectionString;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString.ToString());
    }
}