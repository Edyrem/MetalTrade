using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Services
{
    public class AdvertisementPhotoService
    {
        private readonly AdvertisementPhotoRepository _repository;
        public AdvertisementPhotoService(AdvertisementPhotoRepository repository)
        {
            _repository = repository;
        }
        public async Task Create(AdvertisementPhoto adsPhoto)
        {
            await _repository.CreateAsync(adsPhoto);
            await _repository.SaveChangesAsync();
        }
    }
}
