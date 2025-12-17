using AutoMapper;
using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Services;

public class MetalService : IMetalService
{
    private readonly MetalTypeRepository _repository;
    private readonly IMapper _mapper;
    
    public MetalService(MetalTradeDbContext context, IMapper mapper)
    {
        _repository = new MetalTypeRepository(context);
        _mapper = mapper;
    }
    
    public async Task<MetalTypeDto?> GetAsync(int metalTypeId)
    {
        MetalType? metalType = await _repository.GetAsync(metalTypeId);
        if (metalType == null)
            return null;
        
        MetalTypeDto metalTypeDto = _mapper.Map<MetalTypeDto>(metalType);
        return metalTypeDto;
    }

    public async Task<List<MetalTypeDto>> GetAllAsync()
    {
        IEnumerable<MetalType> metalTypes = await _repository.GetAllAsync();
        return _mapper.Map<List<MetalTypeDto>>(metalTypes);
    }

    public async Task CreateAsync(MetalTypeDto metalTypeDto)
    {
        MetalType metalType = _mapper.Map<MetalType>(metalTypeDto);
        await _repository.CreateAsync(metalType);
        await _repository.SaveChangesAsync();
    }

    public async Task UpdateAsync(MetalTypeDto metalTypeDto)
    {
        MetalType? metalType = _mapper.Map<MetalType>(metalTypeDto);
        await _repository.UpdateAsync(metalType);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int metalTypeId)
    {
        await _repository.DeleteAsync(metalTypeId);
        await _repository.SaveChangesAsync();
    }
}