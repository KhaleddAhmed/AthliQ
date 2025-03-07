using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.Entities.Models;

namespace AthliQ.Core.Repository.Contract
{
    public interface IGenericRepository<T, TKey>
        where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetAsync(TKey id);
        Task AddAsync(T entity);
        Task<IQueryable<T>> GetAllAsyncAsQueryable();
        Task<IQueryable<T>> Get(Expression<Func<T, bool>> predict = null);

        void Update(T entity);
        void Delete(T entity);
    }
}
