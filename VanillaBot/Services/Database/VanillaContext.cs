using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VanillaBot.Services.Database.Models;

namespace VanillaBot.Services.Database
{
    public class VanillaContext : DbContext
    {
        private readonly IConfiguration _config;

        public DbSet<NotificationOpt> NotificationOpts { get; set; }

        public VanillaContext(DbContextOptions<VanillaContext> options, IConfiguration config)
            : base(options)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config["sqlServer"]);
        }
    }
}
