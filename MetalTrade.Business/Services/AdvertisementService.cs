using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly AdvertisementRepository _repository;
        private readonly IMapper _mapper;

        public AdvertisementService(MetalTradeDbContext context, IMapper mapper)
        {
            _repository = new AdvertisementRepository(context);
            _mapper = mapper;
        }

        public async Task<List<AdvertisementDto>> GetAllAsync()
        {
            var ads = await _repository.GetAllAsync();
            return _mapper.Map<List<AdvertisementDto>>(ads);
        }

        public async Task<AdvertisementDto?> GetAsync(int advertisementId)
        {
            var ads = await _repository.GetAsync(advertisementId);
            return _mapper.Map<AdvertisementDto>(ads);
        }

        public async Task CreateAsync(AdvertisementDto adsDto)
        {
            var entity = _mapper.Map<Advertisement>(adsDto);
            await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
        }
        public async Task UpdateAsync(AdvertisementDto adsDto)
        {
            var entity = _mapper.Map<Advertisement>(adsDto);
            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int advertisementId)
        {
            await _repository.DeleteAsync(advertisementId);
            await _repository.SaveChangesAsync();
        }

    }
}
