using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Domain.Entities
{
    public class TopUser: TimedPromotion
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
    }
}
