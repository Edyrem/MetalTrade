using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Abstractions
{
    public abstract class PromotionRepository<T> : Repository<T>, IPromotionRepository<T> where T : TimedPromotion
    {
        protected PromotionRepository(MetalTradeDbContext context) : base(context)
        {
        }

        public virtual async Task AddAsync(T promotion)
        {
            await _dbSet.AddAsync(promotion);
        }

        public virtual async Task<T?> GetActiveAsync()
        {
            return await _dbSet.FirstOrDefaultAsync(c =>
            c.IsActive);
        }

        public virtual async Task<IEnumerable<T>> GetAllActiveAsync()
        {
            return await _dbSet.Where(x => x.IsActive).ToListAsync();
        }

        public virtual async Task<bool> HasActiveAsync()
        {
            return await _dbSet.AnyAsync(p => p.IsActive);
        }
    }
}
