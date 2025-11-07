using MetalTrade.Business.Dtos;

namespace MetalTrade.Business.Interfaces
{
    public interface IAdvertisementService
    {
        Task<AdvertisementDto> GetAsync(int advertisementId);
        Task<List<AdvertisementDto>> GetAllAsync();
        Task<int> CreateAsync(AdvertisementDto advertisement);
        Task UpdateAsync(AdvertisementDto advertisement);
        Task DeleteAsync(int advertisementId);
        Task CreatePhotosAsync(List<AdvertisementPhotoDto> photos);
        Task DeletePhotoAsync(int adsPhotoId);
    }
}
