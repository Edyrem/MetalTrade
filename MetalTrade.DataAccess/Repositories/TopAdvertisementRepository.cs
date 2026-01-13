using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Repositories
{
    public class TopAdvertisementRepository : Repository<TopAdvertisement>, ITopAdvertisementRepository
    {
        public TopAdvertisementRepository(MetalTradeDbContext context) : base(context)
        {
        }

        public async Task<TopAdvertisement?> GetLast(int advertisementId)
        {
            return await _dbSet.LastOrDefaultAsync(t => t.AdvertisementId == advertisementId);
        }
    }
}
