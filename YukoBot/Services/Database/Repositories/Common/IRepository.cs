using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukoBot.Services.Database.Repositories.Common
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }

        void Add(T entity);
        void AddRange(params T[] entities);
        Task AddAsync(T entity);
        Task AddRangeAsync(params T[] entities);

        void Remove(T entity);
        void RemoveRange(params T[] objs);

        void Update(T obj);
        void UpdateRange(params T[] objs);
    }
}
