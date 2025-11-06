using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Services
{
    public class AdvertisementPhotoService
    {
        private readonly AdvertisementPhotoRepository _repository;
        public AdvertisementPhotoService(MetalTradeDbContext context)
        {
            _repository = new AdvertisementPhotoRepository(context);
        }
        public async Task Create(AdvertisementPhoto adsPhoto)
        {
            await _repository.CreateAsync(adsPhoto);
            await _repository.SaveChangesAsync();
        }
    }
}
