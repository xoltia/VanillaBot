using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YukoBot.Services.Database.Repositories.Common
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly YukoContext _dbContext;
        protected DbSet<T> _dbSet => _dbContext.Set<T>();

        public IQueryable<T> Entities => _dbSet;

        public Repository(YukoContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(T entity) =>
            _dbSet.Add(entity);

        public void AddRange(params T[] entities) =>
            _dbSet.AddRange(entities);

        public Task AddAsync(T entity) =>
            _dbSet.AddAsync(entity);

        public Task AddRangeAsync(params T[] entities) =>
            _dbSet.AddRangeAsync(entities);

        public void Remove(T entity) =>
            _dbSet.Remove(entity);

        public void RemoveRange(params T[] entities) =>
            _dbSet.RemoveRange(entities);

        public void Update(T entity) =>
            _dbSet.Update(entity);

        public void UpdateRange(params T[] entities) =>
            _dbSet.UpdateRange(entities);

    }
}
