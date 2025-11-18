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
            adsDto.Product = null;
            adsDto.User = null;

            var entity = _mapper.Map<Advertisement>(adsDto);

            entity.CreateDate = DateTime.UtcNow;

            if (adsDto.Photoes != null && adsDto.Photoes.Any())
            {
                entity.Photoes = adsDto.Photoes.Select(photoDto => new AdvertisementPhoto
                {
                    PhotoLink = photoDto.PhotoLink,
                }).ToList();
            }

            await _repository.CreateAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(AdvertisementDto adsDto)
        {
            var existingEntity = await _repository.GetAsync(adsDto.Id);
            if (existingEntity == null)
                throw new ArgumentException($"Объявление с ID {adsDto.Id} не найдено");

            var currentPhotoes = existingEntity.Photoes?.ToList() ?? new List<AdvertisementPhoto>();

            adsDto.Photoes = null;
            adsDto.Product = null;
            adsDto.User = null;
            _mapper.Map(adsDto, existingEntity);

            if (adsDto.Photoes != null && adsDto.Photoes.Any())
            {
                foreach (var photo in currentPhotoes)
                {
                    photo.IsDeleted = true;
                }

                existingEntity.Photoes = adsDto.Photoes.Select(photoDto => new AdvertisementPhoto
                {
                    PhotoLink = photoDto.PhotoLink
                }).ToList();
            }
            else
            {
                existingEntity.Photoes = currentPhotoes;
            }

            await _repository.UpdateAsync(existingEntity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int advertisementId)
        {
            await _repository.DeleteAsync(advertisementId);
            await _repository.SaveChangesAsync();
        }

    }
}
