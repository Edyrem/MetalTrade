using MetalTrade.DataAccess.Abstractions;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Repositories;

public class CommercialRepository : PromotionRepository<Commercial>, ICommercialRepository
{
    public CommercialRepository(MetalTradeDbContext context) : base(context)
    {
    }

    public async Task<Commercial?> GetLast(int advertisementId)
    {
        return await _dbSet.LastOrDefaultAsync(c => c.AdvertisementId == advertisementId);
    }

    public async Task<bool> HasActiveAsync(int advertisementId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet.AnyAsync(c =>
            c.AdvertisementId == advertisementId &&
            c.StartDate <= now &&
            c.EndDate >= now);
    }

    public async Task<Commercial?> GetActiveAsync(int advertisementId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet.FirstOrDefaultAsync(c =>
            c.AdvertisementId == advertisementId &&
            c.StartDate <= now &&
            c.EndDate >= now);
    }
}

