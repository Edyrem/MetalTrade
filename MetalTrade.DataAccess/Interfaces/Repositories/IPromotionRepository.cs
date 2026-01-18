using MetalTrade.Domain.Abstraction;

namespace MetalTrade.DataAccess.Interfaces.Repositories
{
    public interface IPromotionRepository<T>: IRepository<T> where T : TimedPromotion
    {
        Task AddAsync(T promotion);
        Task<T?> GetActiveAsync();
        Task<IEnumerable<T>> GetAllActiveAsync();
        Task<bool> HasActiveAsync();
    }
}
