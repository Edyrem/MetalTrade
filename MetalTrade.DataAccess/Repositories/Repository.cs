using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MetalTrade.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MetalTradeDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(MetalTradeDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public virtual async Task<T?> GetAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, Boolean>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public virtual async Task CreateAsync(T item)
        {
            await _dbSet.AddAsync(item);
        }
        public virtual async Task UpdateAsync(T item)
        {
            _dbSet.Update(item);
        }
        public virtual async Task DeleteAsync(int id)
        {
            T entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
