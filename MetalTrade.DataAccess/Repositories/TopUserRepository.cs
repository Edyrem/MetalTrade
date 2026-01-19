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
            return await _dbSet.Include(x => x.TargetUser)
                .FirstOrDefaultAsync(t => t.TargetUserId == userId && t.IsActive);
        }

        public async Task<TopUser?> GetLast(int userId)
        {
            return await _dbSet.LastOrDefaultAsync(tu => tu.TargetUserId == userId);
        }

        public async Task<bool> HasActiveAsync(int userId)
        {
            return await _dbSet.AnyAsync(t => t.TargetUserId == userId && t.IsActive);
        }

        public override async Task<IEnumerable<TopUser>> GetAllActiveAsync()
        {
            return await _dbSet.Where(x => x.IsActive).Include(x => x.TargetUser).ToListAsync();
        }
    }
}
