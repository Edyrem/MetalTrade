using MetalTrade.Business.Dtos;

namespace MetalTrade.Business.Interfaces;

public interface ICommercialService
{
    Task ActivateAsync(CommercialDto dto);
    Task<bool> IsActiveAsync(int advertisementId);
    Task DeactivateAsync(int advertisementId);
    Task<bool> HasActiveCommercialAsync(int advertisementId);
    Task<DateTime?> GetActiveAdEndDateAsync(int advertisementId);

}