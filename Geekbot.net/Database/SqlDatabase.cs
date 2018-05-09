using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
            => optionsBuilder.UseLoggerFactory(new LoggerFactory().AddConsole())
                .UseMySql(_connectionString.ToString());
    }
}