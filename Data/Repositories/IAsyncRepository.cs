using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DemoApi.Data.Repositories
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> Where(Expression<Func<T, bool>> predicate);

        void Update(T obj);

        void Delete(T obj);

        Task AddAsync(T obj);

        Task<int> SaveAsync();

        Task<int> CountAllAsync();
    }
}