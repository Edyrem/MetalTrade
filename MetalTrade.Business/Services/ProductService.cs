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

        ProductDto productDto = _mapper.Map<ProductDto>(product);

        return productDto;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        IEnumerable<Product> products = await _repository.GetAllAsync();
        return products.Select(product => new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            MetalTypeId = product.MetalTypeId,
            MetalType = new MetalTypeDto
            {
                Id = product.MetalType.Id,
                Name = product.MetalType.Name
            },
            Advertisements = product.Advertisements
                .Select(a => new AdvertisementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Price = a.Price
                })
                .ToList()
        }).ToList();
        
    }

    public async Task CreateAsync(ProductDto productDto)
    {
        Product product = new Product()
        {
            Id = productDto.Id,
            Name = productDto.Name.ToLower(),
            MetalTypeId = productDto.MetalTypeId
        };
        await _repository.CreateAsync(product);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductDto productDto)
    {
        Product? product = await _repository.GetAsync(productDto.Id);
        if (product == null)
            return;
        
        product.Id = productDto.Id;
        product.Name = productDto.Name.ToLower();
        product.MetalTypeId = productDto.MetalTypeId;
        await _repository.UpdateAsync(product);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int productId)
    {
        await _repository.DeleteAsync(productId);
        await _repository.SaveChangesAsync();
    }
}