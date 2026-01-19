using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Domain.Entities
{
    public class TopUser: TimedPromotion
    {
        public int TargetUserId { get; set; }
        public User TargetUser { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
    }
}
