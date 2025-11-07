using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Interfaces
{
    internal interface IAdvertisementService
    {
        Task GetAsync(int advertisementId);
        Task GetAllAsync();
        Task CreateAsync();
        Task UpdateAsync();
        Task DeleteAsync();
        Task CreatePhotosAsync();
        Task DeletePhotoAsync();
    }
}
