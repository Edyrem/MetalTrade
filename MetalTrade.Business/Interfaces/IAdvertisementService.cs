using MetalTrade.Business.Dtos;
using MetalTrade.Domain.Entities;
using System.Linq.Expressions;

namespace MetalTrade.Business.Interfaces
{
    public interface IAdvertisementService
    {
        Task<AdvertisementDto?> GetAsync(int advertisementId);
        Task<List<AdvertisementDto>> GetAllAsync();
        Task<IEnumerable<AdvertisementDto>> FindAsync(Expression<Func<Advertisement, bool>> predicate);
        Task CreateAsync(AdvertisementDto adsDto);
        Task UpdateAsync(AdvertisementDto adsDto);
        Task DeleteAsync(int advertisementId);
        Task ApproveAsync(int advertisementId);
        Task RejectAsync(int advertisementId); 
        Task ArchiveAsync(int advertisementId);
        Task<bool> DeleteAdvertisementPhotoAsync(AdvertisementPhotoDto advertisementPhotoDto);
        Task<List<AdvertisementDto>> GetFilteredAsync(AdvertisementFilterDto filter);
    }
}