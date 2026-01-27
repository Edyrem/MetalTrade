using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess.Interfaces.Repositories
{
    public interface ITopUserRepository: IPromotionRepository<TopUser>
    {
        Task<bool> HasActiveAsync(int userId);
        Task<TopUser?> GetActiveAsync(int userId);
        Task<TopUser?> GetLast(int userId);
    }
}
