using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess.Interfaces.Repositories;

public interface ICommercialRepository: IPromotionRepository<Commercial>
{
    Task<Commercial?> GetLast(int advertisementId);
    Task<bool> HasActiveAsync(int advertisementId);
    Task<Commercial?> GetActiveAsync(int advertisementId);
}
