using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DemoApi.Data.Repositories
{
    public class Repository<T> : IAsyncRepository<T> where T : class
    {
        protected DataContext Db;
        protected DbSet<T> Table;

        public Repository(DataContext db)
        {
            Db = db;
            db.Database.Migrate();
            Table = db.Set<T>();
        }

        public async Task AddAsync(T obj)
        {
            await Table.AddAsync(obj);
        }

        public async Task<int> CountAllAsync()
        {
            return await Table.CountAsync();
        }

        public void Delete(T obj)
        {
            Table.Remove(obj);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Table.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await Table.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await Table.FindAsync(id);
        }

        public async Task<int> SaveAsync()
        {
            return await Db.SaveChangesAsync();
        }

        public void Update(T obj)
        {
            Db.Update(obj);
        }

        public async Task<List<T>> Where(Expression<Func<T, bool>> predicate)
        {
            return await Table.Where(predicate).ToListAsync();
        }
    }
}