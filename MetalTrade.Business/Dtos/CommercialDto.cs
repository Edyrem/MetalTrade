using MetalTrade.Domain.Entities;

namespace MetalTrade.Business.Dtos;

public class CommercialDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }

    // ID модератора/администратора, который создал промоушен
    public int? CreatedByUserId { get; set; }
    // Навигационное свойство к модератору (lazy loading)
    public UserDto? CreatedBy { get; set; }
    public int AdvertisementId { get; set; }
    public AdvertisementDto Advertisement { get; set; } = null!;
    public decimal Cost { get; set; } = 0;
}