using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.Repository.Contract;
using AthliQ.Repository.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace AthliQ.Repository.Repositories
{
    public class GenericRepository<T, Tkey> : IGenericRepository<T, Tkey>
        where T : class
    {
        private readonly AthliQDbContext _athliQDbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AthliQDbContext athliQDbContext)
        {
            _athliQDbContext = athliQDbContext;
            _dbSet = _athliQDbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _athliQDbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetAsync(Tkey id)
        {
            return await _athliQDbContext.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _athliQDbContext.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _athliQDbContext.Update(entity);
        }

        public void Delete(T entity)
        {
            _athliQDbContext.Remove(entity);
        }

        public async Task<IQueryable<T>> GetAllAsyncAsQueryable() => _dbSet.AsNoTracking();

        public async Task<IQueryable<T>> Get(Expression<Func<T, bool>> predict = null)
        {
            return _dbSet.Where(predict);
        }
    }
}
