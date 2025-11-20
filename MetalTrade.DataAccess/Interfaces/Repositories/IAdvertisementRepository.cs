using MetalTrade.Domain.Entities;
using MetalTrade.Domain.Enums;


namespace MetalTrade.DataAccess.Interfaces.Repositories
{
    public interface IAdvertisementRepository: IRepository<Advertisement>
    {
        Task<AdvertisementStatus> GetStatusAsync(int Id);
        Task SetStatusAsync(int Id, AdvertisementStatus status);
    }
}
