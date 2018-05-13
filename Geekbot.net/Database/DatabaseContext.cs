using Geekbot.net.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Geekbot.net.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<QuoteModel> Quotes { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<GuildsModel> Guilds { get; set; }
        public DbSet<GuildSettingsModel> GuildSettings { get; set; }
        public DbSet<KarmaModel> Karma { get; set; }
        public DbSet<ShipsModel> Ships { get; set; }
        public DbSet<RollsModel> Rolls { get; set; }
        public DbSet<MessagesModel> Messages { get; set; }
        public DbSet<SlapsModel> Slaps { get; set; }
        public DbSet<GlobalsModel> Globals { get; set; }
        public DbSet<RoleSelfServiceModel> RoleSelfService { get; set; }

        // public DbSet<UserSettingsModel> UserSettings { get; set; }
    }
}