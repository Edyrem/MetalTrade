using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Repositories;

public class CommercialRepository : ICommercialRepository
{
    private readonly MetalTradeDbContext _context;

    public CommercialRepository(MetalTradeDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasActiveAsync(int advertisementId, DateTime now)
    {
        return await _context.Commercials.AnyAsync(c =>
            c.AdvertisementId == advertisementId &&
            c.StartDate <= now &&
            c.EndDate >= now);
    }

    public async Task<Commercial?> GetActiveAsync(int advertisementId, DateTime now)
    {
        return await _context.Commercials.FirstOrDefaultAsync(c =>
            c.AdvertisementId == advertisementId &&
            c.StartDate <= now &&
            c.EndDate >= now);
    }

    public async Task AddAsync(Commercial commercial)
    {
        await _context.Commercials.AddAsync(commercial);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Commercial commercial)
    {
        _context.Commercials.Update(commercial);
    }
}

