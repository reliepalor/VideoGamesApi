using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VideoGameApi.Data;
using VideoGameApi.Auth.Interfaces;

namespace VideoGameApi.Api.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly VideoGameDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(VideoGameDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetById(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(string id, T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(string id)
        {
            var entity = await GetById(id);
            if (entity == null) return;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        // EF Core SAFE version (no expression-tree issues)
        public async Task<T?> FindByKeyValue(string key, string value)
        {
            var prop = typeof(T).GetProperty(
                key,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            if (prop == null)
                return null;

            // Run in memory to avoid expression-tree limitations
            var list = await _dbSet.ToListAsync();

            return list.FirstOrDefault(entity =>
            {
                var val = prop.GetValue(entity);
                return val != null && val.ToString() == value;
            });
        }

        public async Task<long> CountAsync()
        {
            return await _dbSet.LongCountAsync();
        }
    }
}
