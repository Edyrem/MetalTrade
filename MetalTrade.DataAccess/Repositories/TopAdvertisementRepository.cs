using MetalTrade.DataAccess.Abstractions;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Repositories
{
    public class TopAdvertisementRepository : PromotionRepository<TopAdvertisement>, ITopAdvertisementRepository
    {
        public TopAdvertisementRepository(MetalTradeDbContext context) : base(context)
        {
        }

        public async Task<TopAdvertisement?> GetActiveAsync(int advertisementId)
        {
            return await _dbSet.Include(x => x.Advertisement)
                .FirstOrDefaultAsync(t => t.AdvertisementId == advertisementId && t.IsActive);
        }

        public async Task<TopAdvertisement?> GetLast(int advertisementId)
        {
            return await _dbSet.LastOrDefaultAsync(t => t.AdvertisementId == advertisementId);
        }

        public override async Task<IEnumerable<TopAdvertisement>> GetAllActiveAsync()
        {
            return await _dbSet.Where(x => x.IsActive).Include(x => x.Advertisement).ToListAsync();
        }

        public async Task<bool> HasActiveAsync(int advertisementId)
        {
            return await _dbSet.AnyAsync(t => t.AdvertisementId == advertisementId && t.IsActive);
        }
    }
}
