using MetalTrade.Business.Dtos;

namespace MetalTrade.Business.Interfaces;

public interface IMetalService
{
    Task<MetalTypeDto?> GetAsync(int metalTypeId);
    Task<List<MetalTypeDto>> GetAllAsync();
    Task CreateAsync(MetalTypeDto metalTypeDto);
    Task UpdateAsync(MetalTypeDto metalTypeDto);
    Task DeleteAsync(int metalTypeId);
    Task<List<MetalTypeDto>> GetFilteredAsync(MetalTypeFilterDto filter);
}