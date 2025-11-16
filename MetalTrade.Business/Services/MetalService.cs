using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Services;

public class MetalService : IMetalService
{
    private readonly MetalTypeRepository _repository;
    
    public MetalService(MetalTradeDbContext context)
    {
        _repository = new MetalTypeRepository(context);
    }
    
    public async Task<MetalTypeDto?> GetAsync(int metalTypeId)
    {
        MetalType? metalType = await _repository.GetAsync(metalTypeId);
        if (metalType == null)
            return null;
        
        MetalTypeDto metalTypeDto = new()
        {
            Id = metalType.Id,
            Name = metalType.Name
        };
        return metalTypeDto;
    }

    public async Task<List<MetalTypeDto>> GetAllAsync()
    {
        IEnumerable<MetalType> metalTypes = await _repository.GetAllAsync();
        return metalTypes.Select(metalType => new MetalTypeDto
        {
            Id = metalType.Id,
            Name = metalType.Name
        }).ToList();
    }

    public async Task CreateAsync(MetalTypeDto metalTypeDto)
    {
        MetalType metalType = new MetalType()
        {
            Id = metalTypeDto.Id,
            Name = metalTypeDto.Name
        };
        await _repository.CreateAsync(metalType);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(MetalTypeDto metalTypeDto)
    {
        MetalType? metalType = await _repository.GetAsync(metalTypeDto.Id);
        if (metalType is null)
            throw new Exception($"Metal type not found by id = {metalTypeDto.Id}");
        
        metalType.Id = metalTypeDto.Id;
        metalType.Name = metalTypeDto.Name;
        
        await _repository.UpdateAsync(metalType);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int metalTypeId)
    {
        await _repository.DeleteAsync(metalTypeId);
        await _repository.SaveChangesAsync();
    }
}