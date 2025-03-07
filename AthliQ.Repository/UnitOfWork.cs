using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core;
using AthliQ.Core.Repository.Contract;
using AthliQ.Repository.Data.Contexts;
using AthliQ.Repository.Repositories;

namespace AthliQ.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AthliQDbContext _dbContext;
        private Hashtable _repositories;

        public UnitOfWork(AthliQDbContext dbContext) // ASK CLR For Creating ObjectFrom DbContext Implicitly
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
        }

        public async Task<int> CompleteAsync() => await _dbContext.SaveChangesAsync();

        public IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : class
        {
            var Key = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(Key))
            {
                var repository = new GenericRepository<TEntity, TKey>(_dbContext);
                _repositories.Add(Key, repository);
            }

            return _repositories[Key] as IGenericRepository<TEntity, TKey>;
        }
    }
}
