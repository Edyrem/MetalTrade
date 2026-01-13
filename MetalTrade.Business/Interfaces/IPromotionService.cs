
namespace MetalTrade.Business.Interfaces
{
    public interface IPromotionService
    {
        Task CreatePromotionAsync(int advertisementId);
        Task DeactivatePromotionAsync(int advertisementId);
        Task CreateUserPromotionAsync(int userId);
        Task DeactivateUserPromotionAsync(int userId);
    }
}
