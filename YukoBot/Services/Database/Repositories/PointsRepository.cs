using Discord;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using YukoBot.Services.Database.Models;
using YukoBot.Services.Database.Repositories.Common;

namespace YukoBot.Services.Database.Repositories
{
    public class PointsRepository : Repository<Points>, IPointsRepository
    {
        public PointsRepository(YukoContext context) : base(context)
        {
        }

        public Task<Points[]> GetTopPointsAsync(int top = 5) =>
            _dbSet.OrderByDescending(p => p.Amount).Take(top).ToArrayAsync();

        public async Task<int> GetPointsAsync(string userId)
        {
            Points points = await _dbSet.SingleOrDefaultAsync(p => p.UserId == userId);
            if (points == null)
                return 0;
            return points.Amount;
        }

        public Task<int> GetPointsAsync(IUser user) =>
            GetPointsAsync(user.Id.ToString());

        public async Task AddPointsAsync(string userId, int amount)
        {
            Points points = await _dbSet.SingleOrDefaultAsync(p => p.UserId == userId);
            if (points == null)
            {
                points = new Points()
                {
                    UserId = userId,
                    Amount = amount
                };
                await AddAsync(points);
            }
            else
            {
                points.Amount += amount;
                Update(points);
            }
        }

        public Task AddPointsAsync(IUser user, int amount) =>
            AddPointsAsync(user.Id.ToString(), amount);

        public async Task AddPointsBulkAsync(string[] ids, int amount)
        {
            foreach (string id in ids)
            {
                await AddPointsAsync(id, amount);
            }
        }
    }
}
