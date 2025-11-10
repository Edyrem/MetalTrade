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
                AdvertisementDto adsDto = new()
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
                    ProductId = ad.ProductId,
                    Product = new()
                    {
                        Id = ad.ProductId,
                        Name = ad.Product?.Name ?? string.Empty
                    }
                };
                foreach (var photo in ad.Photoes)
                {
                    adsDto.Photoes.Add(new AdvertisementPhotoDto
                    {
                        Id = photo.Id,
                        PhotoLink = photo.PhotoLink,
                        AdvertisementId = photo.AdvertisementId
                    });
                }
                adsDtos.Add(adsDto);
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
                ProductId = ads.ProductId,
                Product = new ProductDto
                {
                    Id = ads.ProductId,
                    Name = ads.Product?.Name ?? string.Empty
                }
            };
            foreach (var photo in ads.Photoes)
            {
                adsDto.Photoes.Add( new AdvertisementPhotoDto
                {
                    Id = photo.Id,
                    PhotoLink = photo.PhotoLink,
                    AdvertisementId = photo.AdvertisementId
                });
            }
            return adsDto;
        }

        public async Task CreateAsync(AdvertisementDto adsDto)
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
            if (adsDto.Photoes.Count > 0)
            {
                foreach (var photoDto in adsDto.Photoes)
                {
                    ads.Photoes.Add(new AdvertisementPhoto
                    {
                        PhotoLink = photoDto.PhotoLink,
                        Advertisement = ads
                    });
                }
            }
            await _repository.CreateAsync(ads);
            await _repository.SaveChangesAsync();
        }
        public async Task UpdateAsync(AdvertisementDto adsDto)
        {
            Advertisement? ads = await _repository.GetAsync(adsDto.Id);
            if (ads != null)
            {
                ads.Title = adsDto.Title;
                ads.Body = adsDto.Body;
                ads.Price = adsDto.Price;
                ads.Address = adsDto.Address;
                ads.PhoneNumber = adsDto.PhoneNumber;
                ads.City = adsDto.City;
                ads.ProductId = adsDto.ProductId;
                if (ads.Photoes != null && adsDto.Photoes.Count > 0)
                {
                    var adsPhotoIds = ads.Photoes.Select(x => x.Id).ToHashSet();
                    foreach (var photoDto in adsDto.Photoes)
                    {
                        if (!adsPhotoIds.Contains(photoDto.Id))
                            ads.Photoes.Add(new AdvertisementPhoto { PhotoLink = photoDto.PhotoLink });
                    }
                }
                await _repository.UpdateAsync(ads);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int advertisementId)
        {
            await _repository.DeleteAsync(advertisementId);
            await _repository.SaveChangesAsync();
        }

    }
}
