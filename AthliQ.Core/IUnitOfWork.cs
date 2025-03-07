using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.Repository.Contract;

namespace AthliQ.Core
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync();

        IGenericRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : class;
    }
}
