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

        public async Task<List<AdvertisementDto>> GetAllAsync()
        {
            IEnumerable<Advertisement> ads = await _repository.GetAllAsync();
            List<AdvertisementDto> adsDtos = [];
            foreach (var ad in ads)
            {
                adsDtos.Add(new AdvertisementDto()
                {
                    Id = ad.Id,
                    Title = ad.Title,
                    Body = ad.Body,
                    Price = ad.Price,
                    CreateDate = ad.CreateDate,
                    Address = ad.Address,
                    PhoneNumber = ad.PhoneNumber,
                    City = ad.City,
                    Status = ad.Status,
                    IsTop = ad.IsTop,
                    IsAd = ad.IsAd,
                    Photoes = ad.Photoes
                });
            }
            return adsDtos;
        }

        public async Task<AdvertisementDto?> GetAsync(int advertisementId)
        {
            Advertisement? ads = await _repository.GetAsync(advertisementId);
            if (ads == null)
                return null;
            AdvertisementDto adsDto = new()
            {
                Id = ads.Id,
                Title = ads.Title,
                Body = ads.Body,
                Price = ads.Price,
                CreateDate = ads.CreateDate,
                Address = ads.Address,
                PhoneNumber = ads.PhoneNumber,
                City = ads.City,
                Status = ads.Status,
                IsTop = ads.IsTop,
                IsAd = ads.IsAd,
                Photoes = ads.Photoes
            };
            return adsDto;
        }

        public async Task<int> CreateAsync(AdvertisementDto adsDto)
        {
            Advertisement ads = new()
            {
                Title = adsDto.Title,
                Body = adsDto.Body,
                Price = adsDto.Price,
                Address = adsDto.Address,
                PhoneNumber = adsDto.PhoneNumber,
                City = adsDto.City,
                Status = adsDto.Status,
                IsTop = adsDto.IsTop,
                IsAd = adsDto.IsAd,
                ProductId = adsDto.ProductId,
                UserId = adsDto.UserId
            };
            await _repository.CreateAsync(ads);
            await _repository.SaveChangesAsync();
            return adsDto.Id;
        }
        public async Task UpdateAsync(AdvertisementDto adsDto)
        {
            Advertisement ads = new()
            {
                Id = adsDto.Id,
                Title = adsDto.Title,
                Body = adsDto.Body,
                Price = adsDto.Price,
                Address = adsDto.Address,
                PhoneNumber = adsDto.PhoneNumber,
                City = adsDto.City,
                Status = adsDto.Status,
                IsTop = adsDto.IsTop,
                IsAd = adsDto.IsAd,
                ProductId = adsDto.ProductId,
                UserId = adsDto.UserId
            };
            await _repository.UpdateAsync(ads);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int advertisementId)
        {
            await _repository.DeleteAsync(advertisementId);
            await _repository.SaveChangesAsync();
        }

    }
}
