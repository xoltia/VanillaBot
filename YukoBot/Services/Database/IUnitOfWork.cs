using System;
using System.Threading.Tasks;
using YukoBot.Services.Database.Repositories;

namespace YukoBot.Services.Database
{
    public interface IUnitOfWork : IDisposable
    {
        IPointsRepository Points { get; }
        INotificationRepository Notifications { get; }
        IGameNotificationRepository GameNotifications { get; }
        IGuildConfigRepository GuildConfigs { get; }

        int SaveChages();
        Task<int> SaveChangesAsync();
    }
}
