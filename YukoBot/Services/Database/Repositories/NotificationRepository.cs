using Discord;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using YukoBot.Services.Database.Models;
using YukoBot.Services.Database.Repositories.Common;

namespace YukoBot.Services.Database.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(YukoContext context) : base(context)
        {
        }

        public Task<Notification> GetNotificationAsync(string opterId, string opteeId) =>
            _dbSet.SingleOrDefaultAsync(n => n.ReceiverId == opterId && n.OptedId == opteeId);

        public Task<Notification[]> GetNotificationsAsync(IUser user, bool onlyEnabled = true) =>
            GetNotificationsAsync(user.Id.ToString(), onlyEnabled);

        public Task<Notification[]> GetNotificationsAsync(string userId, bool onlyEnabled = true) =>
            _dbSet.Where(n => n.ReceiverId == userId && (onlyEnabled ? n.Enabled : true)).ToArrayAsync();

        public Task<ulong[]> GetPeopleToNotifyAsync(IUser user, IGuild guild) =>
            GetPeopleToNotifyAsync(user.Id.ToString(), guild.Id.ToString());

        public Task<ulong[]> GetPeopleToNotifyAsync(string userId, string guildId) =>
            _dbSet.Where(n => n.Enabled && n.OptedId == userId && n.GuildId == guildId).Select(n => ulong.Parse(n.ReceiverId)).ToArrayAsync();

        public async Task<UpdateResult> TrySetNotificationEnabledAsync(string opterId, string opteeId, bool enabled)
        {
            Notification notification = await GetNotificationAsync(opterId, opteeId);
            if (notification == null)
                return UpdateResult.DoesNotExist;
            if (notification.Enabled == enabled)
                return UpdateResult.NoChangesMade;

            notification.Enabled = enabled;
            Update(notification);
            return UpdateResult.Success;
        }

        public Task<UpdateResult> TrySetNotificationEnabledAsync(IUser opter, IUser optee, bool enabled) =>
            TrySetNotificationEnabledAsync(opter.Id.ToString(), optee.Id.ToString(), enabled);
    }
}
