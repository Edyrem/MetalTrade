using MetalTrade.Business.Dtos;

namespace MetalTrade.Business.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetAsync(int productId);
    Task<List<ProductDto>> GetAllAsync();
    Task CreateAsync(ProductDto productDto);
    Task UpdateAsync(ProductDto productDto);
    Task DeleteAsync(int productId);
    Task<List<ProductDto>> GetFilteredAsync(ProductFilterDto filter);
}