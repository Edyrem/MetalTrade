using MetalTrade.Business.Dtos;
using MetalTrade.Business.Interfaces;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MetalTrade.Business.Services;

public class CommercialService : ICommercialService
{
    private readonly ICommercialRepository _repository;
    private readonly ILogger<CommercialService> _logger;

    
    public CommercialService(ICommercialRepository repository, ILogger<CommercialService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task ActivateAsync(CommercialDto dto)
    {
        if (dto.Days <= 0)
            throw new InvalidOperationException("Количество дней должно быть больше 0");

        var now = DateTime.UtcNow;

        if (await _repository.HasActiveAsync(dto.AdvertisementId, now))
            throw new InvalidOperationException("Реклама уже активна");

        var commercial = new Commercial
        {
            AdvertisementId = dto.AdvertisementId,
            StartDate = now,
            EndDate = now.AddDays(dto.Days),
            Cost = 0
        };

        await _repository.AddAsync(commercial);
        await _repository.SaveChangesAsync();
        _logger.LogInformation(
            "Реклама активирована: AdId={AdId}, Days={Days}, EndDate={EndDate}",
            dto.AdvertisementId,
            dto.Days,
            commercial.EndDate);
    }
    

    public async Task<bool> IsActiveAsync(int advertisementId)
    {
        return await _repository.HasActiveAsync(advertisementId, DateTime.UtcNow);
    }

    
    public async Task DeactivateAsync(int advertisementId)
    {
        var now = DateTime.UtcNow;

        var activeCommercial = await _repository
                                   .GetActiveAsync(advertisementId, now)
                               ?? throw new InvalidOperationException("Активная реклама не найдена");

        activeCommercial.EndDate = now;

        await _repository.SaveChangesAsync();
        _logger.LogInformation(
            "Реклама отключена: AdId={AdId}",
            advertisementId);
    }

    public async Task<bool> HasActiveCommercialAsync(int advertisementId)
    {
        return await _repository.HasActiveAsync(advertisementId, DateTime.UtcNow);
    }

    public async Task<DateTime?> GetActiveAdEndDateAsync(int advertisementId)
    {
        var now = DateTime.UtcNow;

        var commercial = await _repository
            .GetActiveAsync(advertisementId, now);

        return commercial?.EndDate;
    }
    

}
