using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YukoBot.Services.Database;

namespace YukoBot.Services
{
    public class DbService
    {
        private readonly DbContextOptions<YukoContext> _options;

        public DbService(IConfiguration config)
        {
            var optionsBuilder = new DbContextOptionsBuilder<YukoContext>();
            optionsBuilder.UseSqlServer(config["sqlServer"]);
            _options = optionsBuilder.Options;
        }

        public IUnitOfWork GetDbContext() =>
            new UnitOfWork(new YukoContext(_options));
    }
}
