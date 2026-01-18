using MetalTrade.DataAccess.Abstractions;
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
                .Include(a => a.User)
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
            var advertisementPhoto = 
                await _context.AdvertisementPhotos.FirstOrDefaultAsync(p => p.Id == advertisementPhotoId);
            if (advertisementPhoto != null)
                _context.Remove(advertisementPhoto);
        }        

        public async Task<AdvertisementPhoto?> GetAdvertisementPhotoAsync(int advertisementPhotoId)
        {
            return await _context.AdvertisementPhotos.FirstOrDefaultAsync(p => p.Id == advertisementPhotoId);
        }
        
        public IQueryable<Advertisement> CreateFilter()
        {
            return _context.Advertisements
                .Include(ad => ad.Product)
                .Include(ad => ad.Photoes)
                .AsQueryable();
        }

        public IQueryable<Advertisement> FilterTitle(IQueryable<Advertisement> query, string title)
        {
            return query.Where(ad =>
                ad.Title != null &&
                ad.Title.ToLower().Contains(title.ToLower()));
        }

        public IQueryable<Advertisement> FilterCity(IQueryable<Advertisement> query, string city)
        {
            return query.Where(ad =>
                ad.City != null &&
                ad.City.ToLower().Contains(city.ToLower()));
        }

        public IQueryable<Advertisement> FilterProduct(IQueryable<Advertisement> query, int productId)
        {
            return query.Where(ad => ad.ProductId == productId);
        }

        public IQueryable<Advertisement> FilterMetalType(IQueryable<Advertisement> query, int metalTypeId)
        {
            return query.Where(ad => ad.Product.MetalTypeId == metalTypeId);
        }

        public IQueryable<Advertisement> FilterPriceFrom(IQueryable<Advertisement> query, decimal priceFrom)
        {
            return query.Where(ad => ad.Price >= priceFrom);
        }

        public IQueryable<Advertisement> FilterPriceTo(IQueryable<Advertisement> query, decimal priceTo)
        {
            return query.Where(ad => ad.Price <= priceTo);
        }

        public IQueryable<Advertisement> FilterDateFromUtc(IQueryable<Advertisement> query, DateTime dateFrom)
        {
            return query.Where(ad => ad.CreateDate >= dateFrom);
        }

        
        public IQueryable<Advertisement> FilterDateToUtc(IQueryable<Advertisement> query, DateTime dateTo)
        {
            return query.Where(ad => ad.CreateDate <= dateTo);
        }

        
    }
}
