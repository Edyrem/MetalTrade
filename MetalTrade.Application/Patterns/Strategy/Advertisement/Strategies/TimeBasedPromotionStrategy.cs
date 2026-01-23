using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Application.Patterns.Strategy.Advertisement.Strategies
{
    public class TimeBasedPromotionStrategy : IPromotionStrategy
    {
        public string Name => "Manual";

        public async Task<bool> ShouldBeActiveAsync(TimedPromotion timedPromotion)
        {
            var currentTime = DateTime.UtcNow;
            var isInTimeRange = timedPromotion.IsActive && (currentTime >= timedPromotion.StartDate && currentTime <= timedPromotion.EndDate);
            return await Task.FromResult(isInTimeRange);
        }
    }
}
