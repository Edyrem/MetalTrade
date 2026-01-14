
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Interfaces
{
    public interface IPromotionService
    {
        Task<Advertisement> UpdatePromotionAsync(int advertisementId);
        Task DeactivatePromotionAsync(int advertisementId);
        Task<User> UpdateUserPromotionAsync(int userId);
        Task DeactivateUserPromotionAsync(int userId);
        Task CreateCommercialAdvertisementAsync(Commercial advertisement);
        Task CreateTopAdvertisementAsync(TopAdvertisement advertisement);
        Task CreateTopUserAsync(TopUser topUser);
        Task<List<Advertisement>> GetAllActiveCommercialsAsync();
        Task<List<Advertisement>> GetAllActiveTopAdvertisementsAsync();
        Task<List<User>> GetAllActiveTopUsersAsync();
        Task SaveAllChangesAsync();
    }
}
