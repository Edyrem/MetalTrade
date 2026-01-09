using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
