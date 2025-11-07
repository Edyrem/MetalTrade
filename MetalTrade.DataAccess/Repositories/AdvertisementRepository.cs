using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MetalTrade.DataAccess.Repositories
{
    public class AdvertisementRepository : Repository<Advertisement>, IAdvertisementRepository
    {
        private readonly DbSet<AdvertisementPhoto> _photoDbSet;
        public AdvertisementRepository(MetalTradeDbContext context) : base(context)
        {
            _photoDbSet = context.Set<AdvertisementPhoto>();
        }
        public async override Task<IEnumerable<Advertisement>> GetAllAsync()
        {
            return await _dbSet.Include(a => a.Photoes).ToListAsync();
        }
        public async override Task<Advertisement?> GetAsync(int id)
        {
            return await _dbSet.Include(a => a.Photoes).FirstOrDefaultAsync(a => a.Id == id);
        }
        public async override Task<IEnumerable<Advertisement>> FindAsync(Expression<Func<Advertisement, bool>> predicate)
        {
            return await _dbSet.Include(a => a.Photoes).Where(predicate).ToListAsync();
        }
        public async Task CreatePhotosAsync(List<AdvertisementPhoto> advPhotos)
        {
            if (advPhotos.Count > 0)
                await _photoDbSet.AddRangeAsync(advPhotos);
        }
        public async Task DeletePhotoAsync(int advertisementPhotoId)
        {
            AdvertisementPhoto? photo = await _photoDbSet.FirstOrDefaultAsync(p => p.Id == advertisementPhotoId);
            if (photo != null)
                _photoDbSet.Remove(photo);
        }
    }
}
