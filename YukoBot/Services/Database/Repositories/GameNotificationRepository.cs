using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukoBot.Services.Database.Models;
using YukoBot.Services.Database.Repositories.Common;

namespace YukoBot.Services.Database.Repositories
{
    public class GameNotificationRepository : Repository<GameNotification>, IGameNotificationRepository
    {
        public GameNotificationRepository(YukoContext context) : base(context)
        {
        }

        public Task<GameNotification> GetNotificationAsync(string userId, string game) =>
            _dbSet.SingleOrDefaultAsync(n => n.ReceiverId == userId && n.Game == game);

        public Task<GameNotification[]> GetNotificationsAsync(IUser user) =>
            GetNotificationsAsync(user.Id.ToString());

        public Task<GameNotification[]> GetNotificationsAsync(string userId) =>
            _dbSet.Where(n => n.ReceiverId == userId).ToArrayAsync();

        public Task<ulong[]> GetPeopleToNotifyAsync(string game) =>
            _dbSet.Where(n => n.Game == game).Select(n => ulong.Parse(n.ReceiverId)).ToArrayAsync();
    }
}
