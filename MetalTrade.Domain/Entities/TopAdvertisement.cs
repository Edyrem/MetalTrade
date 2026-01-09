
using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Domain.Entities
{
    public class TopAdvertisement: TimedPromotion
    {
        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
    }
}
