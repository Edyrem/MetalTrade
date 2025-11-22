using MetalTrade.Business.Dtos;

namespace MetalTrade.Business.Interfaces
{
    public interface IAdvertisementService
    {
        Task<AdvertisementDto?> GetAsync(int advertisementId);
        Task<List<AdvertisementDto>> GetAllAsync();
        Task CreateAsync(AdvertisementDto adsDto);
        Task UpdateAsync(AdvertisementDto adsDto);
        Task DeleteAsync(int advertisementId);
        Task ApproveAsync(int advertisementId);
        Task RejectAsync(int advertisementId); 
        Task ArchiveAsync(int advertisementId);
    }
}
