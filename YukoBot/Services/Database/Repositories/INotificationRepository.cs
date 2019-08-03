using Discord;
using System.Threading.Tasks;
using YukoBot.Services.Database.Models;
using YukoBot.Services.Database.Repositories.Common;

namespace YukoBot.Services.Database.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<Notification> GetNotificationAsync(string opterId, string opteeId);
        Task<Notification[]> GetNotificationsAsync(IUser user,bool onlyEnabled = true);
        Task<Notification[]> GetNotificationsAsync(string userId, bool onlyEnabled = true);
        Task<ulong[]> GetPeopleToNotifyAsync(IUser user, IGuild guild);
        Task<ulong[]> GetPeopleToNotifyAsync(string userId, string guildId);

        Task<UpdateResult> TrySetNotificationEnabledAsync(string opterId, string opteeId, bool enabled);
        Task<UpdateResult> TrySetNotificationEnabledAsync(IUser opter, IUser optee, bool enabled);
    }
}
