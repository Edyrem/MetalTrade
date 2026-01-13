using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Repositories
{
    public class TopUserRepository: Repository<TopUser>, ITopUserRepository
    {
        public TopUserRepository(MetalTradeDbContext context) : base(context)
        {
        }

        public async Task<TopUser?> GetLast(int userId)
        {
            return await _dbSet.LastOrDefaultAsync(tu => tu.UserId == userId);
        }
    }
}
