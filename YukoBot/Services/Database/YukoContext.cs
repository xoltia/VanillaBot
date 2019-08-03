using Microsoft.EntityFrameworkCore;
using YukoBot.Services.Database.Models;

namespace YukoBot.Services.Database
{
    public class YukoContext : DbContext
    {
        public DbSet<Points> Points { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<GameNotification> GameNotifications { get; set; }
        public DbSet<GuildConfig> GuildConfigs { get; set; }

        public YukoContext(DbContextOptions<YukoContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Points>()
                .HasIndex(p => p.UserId)
                .IsUnique();
        }
    }
}
