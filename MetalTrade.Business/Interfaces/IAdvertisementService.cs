using MetalTrade.Business.Dtos;

namespace MetalTrade.Business.Interfaces
{
    public interface IAdvertisementService
    {
        Task<AdvertisementDto?> GetAsync(int advertisementId);
        Task<List<AdvertisementDto>> GetAllAsync();
        Task<int> CreateAsync(AdvertisementDto adsDto);
        Task UpdateAsync(AdvertisementDto adsDto);
        Task DeleteAsync(int advertisementId);
        Task CreatePhotosAsync(List<AdvertisementPhotoDto> adsPhotoDtos);
        Task DeletePhotoAsync(int adsPhotoId);
    }
}
