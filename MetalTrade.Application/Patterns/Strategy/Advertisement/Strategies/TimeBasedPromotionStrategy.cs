using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Domain.Abstraction;

namespace MetalTrade.Application.Patterns.Strategy.Advertisement.Strategies
{
    public class TimeBasedPromotionStrategy : IPromotionStrategy
    {
        public string Name => "TimeBased";

        public async Task<bool> ShouldBeActiveAsync(TimedPromotion timedPromotion)
        {
            var currentTime = DateTime.UtcNow;
            var isInTimeRange = currentTime >= timedPromotion.StartDate && currentTime <= timedPromotion.EndDate;
            return await Task.FromResult(isInTimeRange);
        }
    }
}
