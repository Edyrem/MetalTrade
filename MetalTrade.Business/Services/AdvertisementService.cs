using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Services
{
    public class AdvertisementService: IAdvertisementService
    {
        private readonly AdvertisementRepository _repository;
        public AdvertisementService(MetalTradeDbContext context)
        {
            _repository = new AdvertisementRepository(context);
        }

        public Task<List<AdvertisementDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AdvertisementDto> GetAsync(int advertisementId)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateAsync(AdvertisementDto advertisement)
        {
            throw new NotImplementedException();
        }
        public Task UpdateAsync(AdvertisementDto advertisement)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int advertisementId)
        {
            throw new NotImplementedException();
        }

        public Task DeletePhotoAsync(int adsPhotoId)
        {
            throw new NotImplementedException();
        }

        public Task CreatePhotosAsync(List<AdvertisementPhotoDto> photos)
        {
            throw new NotImplementedException();
        }

    }
}
