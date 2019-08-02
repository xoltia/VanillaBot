using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukoBot.Services.Database.Models;
using YukoBot.Services.Database.Repositories.Common;

namespace YukoBot.Services.Database.Repositories
{
    public interface IPointsRepository : IRepository<Points>
    {
        Task<Points[]> GetTopPointsAsync(int top = 5);

        Task<int> GetPointsAsync(string userId);
        Task<int> GetPointsAsync(IUser user);

        Task AddPointsAsync(string userId, int amount);
        Task AddPointsAsync(IUser user, int amount);

        Task AddPointsBulkAsync(string[] ids, int amount);
    }
}
