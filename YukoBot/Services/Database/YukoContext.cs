using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
