using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using MetalTrade.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MetalTrade.DataAccess.Repositories
{
    public class AdvertisementRepository : Repository<Advertisement>, IAdvertisementRepository
    {
        public AdvertisementRepository(MetalTradeDbContext context) : base(context)
        {
        }
        public async override Task<IEnumerable<Advertisement>> GetAllAsync()
        {
            return await _dbSet
                .Include(a => a.Photoes)
                .Include(a => a.Product)
                    .ThenInclude(p => p.MetalType)
                .ToListAsync();
        }
        public async override Task<Advertisement?> GetAsync(int id)
        {
            return await _dbSet
                .Include(a => a.Photoes)
                .Include(a => a.Product)
                    .ThenInclude(p => p.MetalType)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async override Task<IEnumerable<Advertisement>> FindAsync(Expression<Func<Advertisement, bool>> predicate)
        {
            return await _dbSet
                .Include(a => a.Photoes)
                .Include(a => a.Product)
                    .ThenInclude(p => p.MetalType)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<AdvertisementStatus> GetStatusAsync(int Id)
        {
            return (AdvertisementStatus) await _dbSet.Where(x => x.Id == Id).Select(x => x.Status).FirstOrDefaultAsync();
        }

        public async Task SetStatusAsync(int Id, AdvertisementStatus status)
        {
            var ad = await _dbSet.FirstOrDefaultAsync(x => x.Id == Id);
            ad.Status = (int)status;
            _dbSet.Update(ad);
        }

        public async Task DeleteAdvertisementPhotoAsync(int advertisementPhotoId)
        {
            var advertisementPhoto = await _context.AdvertisementPhotos.FirstOrDefaultAsync(a => a.Id == advertisementPhotoId);
            if (advertisementPhoto != null)
                _context.Remove(advertisementPhoto);

        }
    }
}
