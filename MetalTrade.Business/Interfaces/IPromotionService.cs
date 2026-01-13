
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Interfaces
{
    public interface IPromotionService
    {
        Task UpdatePromotionAsync(int advertisementId);
        Task DeactivatePromotionAsync(int advertisementId);
        Task UpdateUserPromotionAsync(int userId);
        Task DeactivateUserPromotionAsync(int userId);
        Task CreateCommercialAdvertisementAsync(Commercial advertisement);
        Task CreateTopAdvertisementAsync(TopAdvertisement advertisement);
        Task CreateTopUserAsync(TopUser topUser);
        Task SaveCommercialAdvertisementAsync();
        Task SaveTopAdvertisementAsync();
        Task SaveTopUserAsync();
    }
}
