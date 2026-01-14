using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess.Interfaces.Repositories
{
    public interface ITopUserRepository: IRepository<TopUser>
    {
        Task<TopUser?> GetLast(int userId);
        Task<IEnumerable<TopUser>> GetAllActiveAsync();
    }
}
