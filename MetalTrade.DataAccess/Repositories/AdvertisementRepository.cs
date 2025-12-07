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
        public async Task<AdvertisementPhoto?> GetAdvertisementPhotoAsync(int advertisementPhotoId)
        {
            return await _context.AdvertisementPhotos.FirstOrDefaultAsync(p => p.Id == advertisementPhotoId);
        }
        
        public IQueryable<Advertisement> GetFilteredQueryable(AdvertisementFilter filter)
        {
            var q = _context.Advertisements
                .Include(a => a.Product)
                .Include(a => a.Photoes)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Title))
                q = q.Where(a => a.Title.Contains(filter.Title));

            if (!string.IsNullOrWhiteSpace(filter.City))
                q = q.Where(a => a.City == filter.City);

            if (filter.MetalTypeId.HasValue)
                q = q.Where(a => a.Product.MetalTypeId == filter.MetalTypeId.Value);

            if (filter.PriceFrom.HasValue)
                q = q.Where(a => a.Price >= filter.PriceFrom.Value);

            if (filter.PriceTo.HasValue)
                q = q.Where(a => a.Price <= filter.PriceTo.Value);

            if (filter.DateFromUtc.HasValue)
                q = q.Where(a => a.CreateDate >= filter.DateFromUtc.Value);

            if (filter.DateToUtc.HasValue)
                q = q.Where(a => a.CreateDate <= filter.DateToUtc.Value);
            
            return q;
        }
        public async Task<int> GetFilteredCountAsync(AdvertisementFilter filter)
        {
            return await GetFilteredQueryable(filter).CountAsync();
        }
    }
}
