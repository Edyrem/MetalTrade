using MetalTrade.Domain.Entities;
using MetalTrade.Domain.Enums;


namespace MetalTrade.DataAccess.Interfaces.Repositories
{
    public interface IAdvertisementRepository: IRepository<Advertisement>
    {
        Task<AdvertisementStatus> GetStatus(int Id);
        Task SetStatus(int Id, AdvertisementStatus status);
    }
}
