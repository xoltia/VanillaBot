using System;
using System.Threading.Tasks;
using YukoBot.Services.Database.Repositories;

namespace YukoBot.Services.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        public YukoContext _context;

        public UnitOfWork(YukoContext context)
        {
            _context = context;
        }

        private IPointsRepository _points;
        public IPointsRepository Points => _points ?? (_points = new PointsRepository(_context));

        private INotificationRepository _notifications;
        public INotificationRepository Notifications => _notifications ?? (_notifications = new NotificationRepository(_context));

        private IGameNotificationRepository _gameNotifications;
        public IGameNotificationRepository GameNotifications => _gameNotifications ?? (_gameNotifications = new GameNotificationRepository(_context));

        private IGuildConfigRepository _guildConfigs;
        public IGuildConfigRepository GuildConfigs => _guildConfigs ?? (_guildConfigs = new GuildConfigRepository(_context));

        public int SaveChages() =>
            _context.SaveChanges();

        public Task<int> SaveChangesAsync() =>
            _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
