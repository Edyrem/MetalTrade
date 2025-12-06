using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Services;

public class ProductService : IProductService
{
    private readonly ProductRepository _repository;
    private readonly IMapper _mapper;

    public ProductService(MetalTradeDbContext context, IMapper mapper)
    {
        _repository = new ProductRepository(context);
        _mapper = mapper;
    }

    public async Task<ProductDto?> GetAsync(int productId)
    {
        Product? product = await _repository.GetAsync(productId);
        if (product == null)
            return null;

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        IEnumerable<Product> products = await _repository.GetAllAsync();
        return _mapper.Map<List<ProductDto>>(products);        
    }

    public async Task CreateAsync(ProductDto productDto)
    {
        Product product = _mapper.Map<Product>(productDto);
        await _repository.CreateAsync(product);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductDto productDto)
    {
        Product? product =  _mapper.Map<Product>(productDto);
        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int productId)
    {
        await _repository.DeleteAsync(productId);
        await _repository.SaveChangesAsync();
    }
}