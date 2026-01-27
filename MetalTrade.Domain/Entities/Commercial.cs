using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Domain.Entities;

public class Commercial: TimedPromotion
{
    public int AdvertisementId { get; set; }
    public Advertisement Advertisement { get; set; } = null!;
    public decimal Cost { get; set; } = 0;
}
