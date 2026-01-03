using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

    public async Task<List<ProductDto>> GetFilteredAsync(ProductFilterDto filter)
    {
        var queryableProducts = _repository.CreateFilter();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            if (int.TryParse(filter.Name, out var productId))
            {
                queryableProducts = queryableProducts
                    .Where(p => p.Id == productId);
            }
            else
            {
                queryableProducts = _repository
                    .FilterName(queryableProducts, filter.Name);
            }
        }

        if (filter.MetalTypeId.HasValue)
            queryableProducts = _repository
                .FilterMetalType(queryableProducts, filter.MetalTypeId.Value);

        queryableProducts = filter.Sort switch
        {
            "name_asc" => queryableProducts.OrderBy(p => p.Name),
            "name_desc" => queryableProducts.OrderByDescending(p => p.Name),
            "metal_asc" => queryableProducts.OrderBy(p => p.MetalType.Name),
            "metal_desc" => queryableProducts.OrderByDescending(p => p.MetalType.Name),
            _ => queryableProducts.OrderBy(p => p.Id)
        };

        queryableProducts = queryableProducts
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        var products = await queryableProducts.ToListAsync();
        return _mapper.Map<List<ProductDto>>(products);
    }
}