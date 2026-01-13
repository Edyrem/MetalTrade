using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Repositories;

public class CommercialRepository : Repository<Commercial>, ICommercialRepository
{
    public CommercialRepository(MetalTradeDbContext context) : base(context)
    {
    }

    public async Task<Commercial?> GetLast(int advertisementId)
    {
        return await _dbSet.LastOrDefaultAsync(c => c.AdvertisementId == advertisementId);
    }

    public async Task<bool> HasActiveAsync(int advertisementId, DateTime now)
    {
        return await _dbSet.AnyAsync(c =>
            c.AdvertisementId == advertisementId &&
            c.StartDate <= now &&
            c.EndDate >= now);
    }

    public async Task<Commercial?> GetActiveAsync(int advertisementId, DateTime now)
    {
        return await _dbSet.FirstOrDefaultAsync(c =>
            c.AdvertisementId == advertisementId &&
            c.StartDate <= now &&
            c.EndDate >= now);
    }

    public async Task AddAsync(Commercial commercial)
    {
        await _dbSet.AddAsync(commercial);
    }

    public async Task<IEnumerable<Commercial>> GetAllActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet.Where(x => x.StartDate < now && x.EndDate > now).ToListAsync();
    }
}

