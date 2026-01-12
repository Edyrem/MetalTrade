using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess.Interfaces.Repositories;

public interface ICommercialRepository: IRepository<Commercial>
{
    Task<bool> HasActiveAsync(int advertisementId, DateTime now);

    Task<Commercial?> GetActiveAsync(int advertisementId, DateTime now);

    Task<IEnumerable<Commercial>> GetAllActiveAsync();

    Task AddAsync(Commercial commercial);
    Task UpdateAsync(Commercial commercial);

    Task SaveChangesAsync();

}
