using Discord;
using System.Threading.Tasks;
using YukoBot.Services.Database.Models;
using YukoBot.Services.Database.Repositories.Common;

namespace YukoBot.Services.Database.Repositories
{
    public interface IGameNotificationRepository : IRepository<GameNotification>
    {
        Task<GameNotification> GetNotificationAsync(string userId, string game);
        Task<GameNotification[]> GetNotificationsAsync(IUser user);
        Task<GameNotification[]> GetNotificationsAsync(string userId);

        Task<ulong[]> GetPeopleToNotifyAsync(string game);
    }
}
