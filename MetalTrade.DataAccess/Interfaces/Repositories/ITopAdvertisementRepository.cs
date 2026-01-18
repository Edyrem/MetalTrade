using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess.Interfaces.Repositories
{
    public interface ITopAdvertisementRepository: IPromotionRepository<TopAdvertisement>
    {
        Task<TopAdvertisement?> GetActiveAsync(int advertisementId);
        Task<TopAdvertisement?> GetLast(int advertisementId);
        Task<bool> HasActiveAsync(int advertisementId);
    }
}
