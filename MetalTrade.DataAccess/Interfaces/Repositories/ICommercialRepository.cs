using MetalTrade.Domain.Entities;

namespace MetalTrade.DataAccess.Interfaces.Repositories;

public interface ICommercialRepository
{
    Task<bool> HasActiveAsync(int advertisementId, DateTime now);

    Task<Commercial?> GetActiveAsync(int advertisementId, DateTime now);

    Task AddAsync(Commercial commercial);

    Task SaveChangesAsync();

}
