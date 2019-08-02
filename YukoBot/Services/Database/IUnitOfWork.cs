using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukoBot.Services.Database.Models;
using YukoBot.Services.Database.Repositories;
using YukoBot.Services.Database.Repositories.Common;

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
