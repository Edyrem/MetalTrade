using MetalTrade.DataAccess.Abstractions;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Repositories
{
    public class TopUserRepository: PromotionRepository<TopUser>, ITopUserRepository
    {
        public TopUserRepository(MetalTradeDbContext context) : base(context)
        {
        }

        public async Task<TopUser?> GetActiveAsync(int userId)
        {
            return await _dbSet.Include(x => x.User)
                .FirstOrDefaultAsync(t => t.UserId == userId && t.IsActive);
        }

        public async Task<TopUser?> GetLast(int userId)
        {
            return await _dbSet.LastOrDefaultAsync(tu => tu.UserId == userId);
        }

        public async Task<bool> HasActiveAsync(int userId)
        {
            return await _dbSet.AnyAsync(t => t.UserId == userId && t.IsActive);
        }
    }
}
