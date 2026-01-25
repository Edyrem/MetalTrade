
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Interfaces
{
    public interface IPromotionService
    {
        // Create Promotions
        Task CreateCommercialAdvertisementAsync(Commercial advertisement);
        Task CreateTopAdvertisementAsync(TopAdvertisement advertisement);
        Task CreateTopUserAsync(TopUser topUser);

        // Update Promotions
        Task UpdatePromotionAsync(int advertisementId);
        Task UpdateUserPromotionAsync(int userId);

        // Deactivate Promotions
        Task DeactivatePromotionAsync(int advertisementId, string? type = null);
        Task DeactivateUserPromotionAsync(int userId);


        Task<IEnumerable<Advertisement>> GetAllActiveCommercialsAsync();
        Task<IEnumerable<Advertisement>> GetAllActiveTopAdvertisementsAsync();
        Task<IEnumerable<User>> GetAllActiveTopUsersAsync();

        Task SaveAllChangesAsync();
    }
}
