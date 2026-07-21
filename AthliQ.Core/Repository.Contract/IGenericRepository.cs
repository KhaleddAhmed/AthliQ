using System.Linq.Expressions;

namespace AthliQ.Core.Repository.Contract
{
    public interface IGenericRepository<T, TKey>
        where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetAsync(TKey id);
        Task AddAsync(T entity);
        Task<IQueryable<T>> GetAllAsyncAsQueryable();
        IQueryable<T> Get(Expression<Func<T, bool>> predict = null);
        void Update(T entity);
        void Delete(T entity);
    }
}
