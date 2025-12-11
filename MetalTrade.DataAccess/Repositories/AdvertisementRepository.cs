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
            var advertisementPhoto = await _dbSet.FirstOrDefaultAsync(a => a.Id == advertisementPhotoId);
            if (advertisementPhoto != null)
                _context.Remove(advertisementPhoto);
        }        
        
        public IQueryable<Advertisement> CreateFilter()
        {
            return _context.Advertisements
                .Include(q => q.Product)
                .Include(q => q.Photoes)
                .AsQueryable();
        }

        public IQueryable<Advertisement> FilterTitle(IQueryable<Advertisement> query, string title)
        {
            var lowered = title.ToLower();
            return query.Where(q => q.Title.ToLower().Contains(lowered));
        }

        public IQueryable<Advertisement> FilterCity(IQueryable<Advertisement> query, string city)
        {
            var c = city.ToLower();
            return query.Where(q => q.City != null && q.City.ToLower().Contains(c));
        }

        public IQueryable<Advertisement> FilterProduct(IQueryable<Advertisement> query, int productId)
        {
            return query.Where(q => q.ProductId == productId);
        }

        public IQueryable<Advertisement> FilterMetalType(IQueryable<Advertisement> query, int metalTypeId)
        {
            return query.Where(q => q.Product.MetalTypeId == metalTypeId);
        }

        public IQueryable<Advertisement> FilterPriceFrom(IQueryable<Advertisement> query, decimal priceFrom)
        {
            return query.Where(q => q.Price >= priceFrom);
        }

        public IQueryable<Advertisement> FilterPriceTo(IQueryable<Advertisement> query, decimal priceTo)
        {
            return query.Where(q => q.Price <= priceTo);
        }

        public IQueryable<Advertisement> FilterDateFromUtc(IQueryable<Advertisement> query, DateTime dateFrom)
        {
            return query.Where(q => q.CreateDate >= dateFrom);
        }

        public IQueryable<Advertisement> FilterDateToUtc(IQueryable<Advertisement> query, DateTime dateTo)
        {
            return query.Where(q => q.CreateDate <= dateTo);
        }

    }
}
